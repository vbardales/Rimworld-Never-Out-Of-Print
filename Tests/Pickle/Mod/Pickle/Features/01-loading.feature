Feature: Never Out of Print loads with its defs wired

  # The offline harness (Tests/test_resources.py) proves the XML is well formed and the recipe/def
  # wiring resolves on paper. Only a started game proves the loader admitted the mod, no patch threw,
  # and the def database it actually built has everything the recipe scenario below will need.

  Scenario: the mod loads without startup errors
    Then mod "nelim.neveroutofprint" is loaded
    And no errors were logged

  Scenario: the presses, the recipe and the research project exist
    Then def "NeverOutOfPrint_Manual" of type "ThingDef" exists
    And def "NeverOutOfPrint_Electric" of type "ThingDef" exists
    And def "NeverOutOfPrint_CopyBook" of type "RecipeDef" exists
    And def "NeverOutOfPrint_Printing" of type "ResearchProjectDef" exists
    And def "NeverOutOfPrint_Manual" costs 30 "Steel"

  Scenario: the copy patch reached the book comp and the market value stat
    Then def "Novel" was patched
    And def "MarketValueBase" was patched by mod "nelim.neveroutofprint"
