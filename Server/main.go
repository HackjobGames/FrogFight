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
		i, e := strconv.Atoi(req.FormValue("relayID"))
		if(e != nil) {
			fmt.Println(e.Error())
		} else {
			fmt.Println(i)
			newMatch := Match{
				matchID: generateMatchID(),
				relayID: i,
			}
			matches = append(matches, newMatch)
			io.WriteString(res, newMatch.matchID)
		}
	})

	http.HandleFunc("/join", func(res http.ResponseWriter, req *http.Request) {
		req.FormValue("matchID")
		for i := range matches {
			if (matches[i].matchID == req.FormValue("matchID")) {
				print(matches[i].relayID)
				s := strconv.Itoa(matches[i].relayID)
				io.WriteString(res, s)
			}
		}
	})

	http.ListenAndServe(":8090", nil)
}
