package main

import (
	"fmt"
	"net/http"
	"strconv"
	"io"
	"math/rand"
)

type Match struct {
	matchID string
	relayID int
	private bool
	password string
	maxPlayers int
	currentPlayers int
}

func generateMatchID() string {
	var letterRunes = []rune("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ")
	ret := make([]rune, 6)
	for i := range ret {
		ret[i] = letterRunes[rand.Intn(len(letterRunes))]
	}
	return string(ret)
}

func main() {
	var matches []Match
	
	http.HandleFunc("/host", func(res http.ResponseWriter, req *http.Request) {
		fmt.Println(req.FormValue("relayID"))
		relayID, rE := strconv.Atoi(req.FormValue("relayID"))
		maxPlayers, mE := strconv.Atoi(req.FormValue("maxPlayers"))
		isPrivate := req.FormValue("isPrivate")
		password := req.FormValue("password")
		fmt.Println("Password: " + password)
		if (rE != nil) {
			fmt.Println(rE.Error())
			return
		}
		if (mE != nil) {
			fmt.Println(mE.Error())
			return
		}
		fmt.Println(relayID)
		newMatch := Match{
			matchID: generateMatchID(),
			relayID: relayID,
			private: isPrivate == "true",
			password: password,
			maxPlayers: maxPlayers,
			currentPlayers: 1,
		}
		fmt.Println(newMatch.private)
		matches = append(matches, newMatch)
		io.WriteString(res, newMatch.matchID)
	})

	http.HandleFunc("/join", func(res http.ResponseWriter, req *http.Request) {
		matchID := req.FormValue("matchID")
		password := req.FormValue("password")
		for i := range matches {
			if (matches[i].matchID == matchID) {
				if (!matches[i].private || matches[i].password == password) {
					io.WriteString(res, strconv.Itoa(matches[i].relayID))
				} else {
					io.WriteString(res, "private");
				}
			}
		}
	})

	http.ListenAndServe(":8090", nil)
}
