package main

import (
	"encoding/json"
	"fmt"
	"io"
	"math/rand"
	"net/http"
	"strconv"
	"time"
	"os"
	"database/sql"
	_ "github.com/lib/pq"
)

var (
	host = "localhost"
	port = 5432
	dbname = "Frog"
	user = os.Getenv("PGUSER")
	password = os.Getenv("PGPASSWORD")
)


type Match struct {
	MatchID        string
	HostName			 string
	Private        bool
	MaxPlayers     int
	password			 string
	relayID				 int
	CurrentPlayers int
}

func generateMatchID() string {
	var letterRunes = []rune("0123456789")
	ret := make([]rune, 6)
	for i := range ret {
		ret[i] = letterRunes[rand.Intn(len(letterRunes))]
	}
	return string(ret)
}

func cleanUp(db *sql.DB) <-chan int {
	r := make(chan int)
	for {
		time.Sleep(60 * time.Second)
		_, err := db.Exec("delete from matches where DateDiff('', lastused::time, now()::time) > 3600")
		if err != nil {
			fmt.Println(err.Error())
			return r
		}
	}
}

func main() {
  sqlParams := fmt.Sprintf("host=%s port=%d user=%s "+
			"password=%s dbname=%s sslmode=disable",
			host, port, user, password, dbname)
	db, err := sql.Open("postgres", sqlParams)
	if err != nil {
		panic(err)
	}
	rand.Seed(time.Now().UTC().UnixNano())
	http.HandleFunc("/host", func(res http.ResponseWriter, req *http.Request) {
		fmt.Println(req.FormValue("relayID"))
		relayID, rE := strconv.Atoi(req.FormValue("relayID"))
		maxPlayers, mE := strconv.Atoi(req.FormValue("maxPlayers"))
		isPrivate := req.FormValue("isPrivate")
		password := req.FormValue("password")
		hostName := req.FormValue("hostName")
		if rE != nil {
			fmt.Println(rE.Error())
			return
		}
		if mE != nil {
			fmt.Println(mE.Error())
			return
		}
		ID := ""
		for {
			ID = generateMatchID()
			sql := fmt.Sprintf(`insert into public.matches (matchid, hostname, relayid, private, password, maxplayers, currentplayers)
							values ('%s', '%s', %d, %s, '%s', %d, 1)`, ID, hostName, relayID, isPrivate, password, maxPlayers)
			_, err = db.Exec(sql)
			if err != nil {
				fmt.Println(err.Error())
			} else {
				break
			}
		}
		io.WriteString(res, ID)
	})

	http.HandleFunc("/join", func(res http.ResponseWriter, req *http.Request) {
		matchID := req.FormValue("matchID")
		password := req.FormValue("password")
		var match Match
		row := db.QueryRow("select matchid, hostname, private, password, relayid, maxplayers, currentplayers from public.matches where matchid = $1", matchID)
		row.Scan(&match.MatchID, &match.HostName, &match.Private, &match.password, &match.relayID, &match.MaxPlayers, &match.CurrentPlayers)
		fmt.Println(match.MatchID)
		if !(match.MatchID == ""){
			if !match.Private || match.password == password {
				if match.CurrentPlayers < match.MaxPlayers {
					io.WriteString(res, strconv.Itoa(match.relayID))
					_, err := db.Exec("update matches set currentplayers = currentplayers + 1, lastused = NOW() where matchid = $1", matchID)
					if err != nil {
						fmt.Println(err.Error())
					}
				} else {
					res.WriteHeader(http.StatusForbidden)
					io.WriteString(res, "Match Is Full")
				}
			} else {
				io.WriteString(res, "private")
			}
		} else {
			res.WriteHeader(http.StatusNotFound)
			io.WriteString(res, "Invalid MatchId")
		}
	})

	http.HandleFunc("/getMatches", func(res http.ResponseWriter, req *http.Request) {
		var matches []Match
		rows, err := db.Query("select matchid, hostname, private, maxplayers, currentplayers from public.matches")

		if err != nil {
			res.WriteHeader(http.StatusInternalServerError)
			io.WriteString(res, "ServerError")
			rows.Close()
			return
		}

		for rows.Next() {
			var match Match
			err := rows.Scan(&match.MatchID, &match.HostName, &match.Private, &match.MaxPlayers, &match.CurrentPlayers)
			if (err != nil) {
				res.WriteHeader(http.StatusInternalServerError)
				io.WriteString(res, "ServerError")
				rows.Close()
			}
			matches = append(matches, match)
		}

		jsonString, err2 := json.Marshal(matches)

		if err2 != nil {
			res.WriteHeader(http.StatusInternalServerError)
			io.WriteString(res, "ServerError")
			rows.Close()
			return
		}

		fmt.Println(len(matches))

		if err != nil {
			fmt.Println(err.Error())
			return
		}
		res.Header().Set("Content-Type", "application/json")
		res.Write(jsonString)
	})

	http.HandleFunc("/removeMatch", func(res http.ResponseWriter, req *http.Request) {
		matchID := req.FormValue("matchID")
		_, err := db.Exec("delete from matches where matchid = $1", matchID)
		fmt.Println("Remove: " + matchID)
		if err == nil {
			res.WriteHeader(http.StatusOK)
		} else {
			fmt.Println(err.Error())
			res.WriteHeader(http.StatusInternalServerError)
		}
	})

	http.HandleFunc("/removePlayer", func(res http.ResponseWriter, req *http.Request) {
		matchID := req.FormValue("matchID")
		db.Exec("update matches set currentplayers = currentplayers - 1 where matchid = $1", matchID)
		fmt.Println("Remove Player From Match: ", matchID)
	})

	http.HandleFunc("/ping", func(res http.ResponseWriter, req *http.Request) {
		matchID := req.FormValue("matchID")
		db.Exec("update matches set lastused = NOW() where matchid = $1", matchID)
	})

	go cleanUp(db)
	fmt.Println("Listening...")
	http.ListenAndServe(":8090", nil)
	




}
