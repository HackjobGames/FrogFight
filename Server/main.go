package main

import (
	"encoding/json"
	"fmt"
	"io"
	"math/rand"
	"net/http"
	"strconv"
	"time"
)

type Match struct {
	MatchID        string
	HostName			 string
	relayID        int
	Private        bool
	password       string
	MaxPlayers     int
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

func main() {
	rand.Seed(time.Now().UTC().UnixNano())
	matches := map[string]Match{}
	matchesPointer := &matches
	http.HandleFunc("/host", func(res http.ResponseWriter, req *http.Request) {
		fmt.Println(req.FormValue("relayID"))
		relayID, rE := strconv.Atoi(req.FormValue("relayID"))
		maxPlayers, mE := strconv.Atoi(req.FormValue("maxPlayers"))
		isPrivate := req.FormValue("isPrivate")
		password := req.FormValue("password")
		hostName := req.FormValue("hostName")
		fmt.Println("Password: " + password)
		fmt.Println(maxPlayers)
		if rE != nil {
			fmt.Println(rE.Error())
			return
		}
		if mE != nil {
			fmt.Println(mE.Error())
			return
		}
		fmt.Println(relayID)
		ID := ""
		for {
			ID = generateMatchID()
			_, found := matches[ID]
			if !found {
				break
			}
		}

		newMatch := Match{
			MatchID:        ID,
			HostName: 			hostName,
			relayID:        relayID,
			Private:        isPrivate == "true",
			password:       password,
			MaxPlayers:     maxPlayers,
			CurrentPlayers: 1,
		}
		fmt.Println(newMatch.Private)
		matches[newMatch.MatchID] = newMatch
		io.WriteString(res, newMatch.MatchID)
	})

	http.HandleFunc("/join", func(res http.ResponseWriter, req *http.Request) {
		matchID := req.FormValue("matchID")
		password := req.FormValue("password")
		match, exists := matches[matchID]
		fmt.Println(match.CurrentPlayers)
		fmt.Println(match.MaxPlayers)
		if exists {
			if !match.Private || match.password == password {
				if match.CurrentPlayers < match.MaxPlayers {
					io.WriteString(res, strconv.Itoa(match.relayID))
					match.CurrentPlayers = match.CurrentPlayers + 1
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
		jsonString, err := json.Marshal(matchesPointer)
		if err != nil {
			fmt.Println(err.Error())
			return
		}
		res.Header().Set("Content-Type", "application/json")
		res.Write(jsonString)
	})

	http.HandleFunc("/removeMatch", func(res http.ResponseWriter, req *http.Request) {
		matchID := req.FormValue("matchID")
		_, ok := matches[matchID]
		fmt.Println("Remove: " + matchID)
		if ok {
			delete(matches, matchID)
		} else {
			res.WriteHeader(http.StatusNotFound)
			io.WriteString(res, "Invalid MatchId")
		}
	})

	http.HandleFunc("/removePlayer", func(res http.ResponseWriter, req *http.Request) {
		matchID := req.FormValue("matchID")
		match, ok := matches[matchID]
		if ok {
			match.CurrentPlayers--
		}
		fmt.Println("Remove Player From Match: ", matchID)
		fmt.Println("Current Players: ", match.CurrentPlayers)
	})

	http.ListenAndServe(":8090", nil)
	fmt.Println("Listening...")
}
