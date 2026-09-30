# TESTING.md's bill-lifecycle scenario, automated. Steel is spent from nothing by dev-mode spawning
# rather than mined, since the point here is the recipe's outcome, not the colony economy.

Feature: a printing press copies a book

  Background:
    Given the save "test-colony" is loaded
    And game speed is paused
    And I close all dialogs
    And god mode is enabled
    And research "NeverOutOfPrint_Printing" is finished

  Scenario: a copy survives the original, is marked, and is worth a tenth to a trader
    Given a colonist "Tester" exists
    And I set "Tester" priority "Crafting" to 1
    And a "NeverOutOfPrint_Manual" is built at (10, 10)
    And 1 "Novel" is spawned at the stockpile
    And 100 "Cloth" is spawned at the stockpile
    When I add bill "NeverOutOfPrint_CopyBook" to the "NeverOutOfPrint_Manual"
    And I wait 12500 ticks
    Then 2 "Novel" exist
    And the newest "Novel" on the map is marked as a printed copy
    And the newest "Novel" on the map is worth about a tenth of the oldest one to a trader
    And no errors were logged
