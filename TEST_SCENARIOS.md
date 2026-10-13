# Test scenarios

One scenario per behaviour promised in `PUBLICATION.md` and the Steam description. Figures checked against the code (`Source/`, `Mod/Defs/`). Played in game by `playTests`, through Pickle where it fits (`Tests/Pickle/`), otherwise by the owner.

| # | Behaviour | Preconditions | Actions | Expected |
|---|---|---|---|---|
| 1 | Two presses behind research | Core, Harmony, this mod | Open the research tree; build each press | A printing project unlocks both; manual press costs 150 wood or metal; electric press needs power and works four times as fast |
| 2 | Copy book keeps the original | A colony holding one book, a press, 100 cloth | Add a Copy book bill, pick the book in the ingredient filter, finish it | The original returns to the shelf; one new copy exists; 100 cloth spent |
| 3 | Copy carries what the original teaches | A skill book, a research book | Copy each, read both | Skill rates, research subjects, joy factor, mental-break chance and quality are identical |
| 4 | No second one-off discovery | An Odyssey map (needs Odyssey) | Copy it | The copy is marked as a copy and holds no second treasure quest |
| 5 | Fewest-copies bill | Three titles held in 1, 2 and 5 copies | Add a Copy book with fewest copies bill, target 3 | The press prints the title with the fewest copies first and stops at 3 each |
| 6 | Copies are marked and cheap | A printed copy | Inspect it; read the stats report; value it to a trader | Says it is a copy; worth one tenth of the original |
| 7 | Stockpile filters | A stockpile | Toggle allow copied books and allow original books | Only the allowed kind is accepted |
| 8 | Works with other books | Vanilla Books Expanded loaded (optional) | Copy one of its books | Copy works like any base-game book |
| 9 | Ideoligion book, own faith | Ideology; a believer | Read an ideoligion book of their own faith | Certainty rises; the blurb says which way it pushes |
| 10 | Ideoligion book, rival faith | Ideology; a believer of another faith | Order the read | Certainty falls; at zero the reader converts through the game's conversion path |
| 11 | Awful quality inverts | Ideology; an awful-quality ideoligion book | Read it as a believer | Certainty falls (not converted to a Pickle step yet, see `TESTING.md`) |
| 12 | Colonists do not seek a rival book | Ideology; two factions | Wait | No colonist picks a rival-faith book on their own |
| 13 | Reading policy effect | Ideology | Open a reading policy | An ideoligion effect sits beside skill gain and research |
| 14 | Burn books | A campfire or crematorium, a book | Add the burn bill | The book burns; recipe exists wherever burning drugs does |
| 15 | Loads without errors, FR and EN | Core + this mod, then with Ideology, Odyssey, Vanilla Books Expanded | Start a game in each language | No red errors in `Player.log`; no raw keys on screen |
