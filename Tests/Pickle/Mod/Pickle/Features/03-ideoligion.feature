# TESTING.md's ideoligion-persuasion scenario, automated. BookOutcomeDoer_Ideoligion.OnReadingTick is
# called directly, the same method RimWorld.JobDriver_Reading ticks during a real read: a scenario
# still needs a live game (a generated Pawn and its Ideo, only Ideology can build), but does not need
# to wait out a realistic multi-day read to reach the outcome.

@requires:Ludeon.RimWorld.Ideology
Feature: an ideoligion book moves a reader's certainty

  Background:
    Given the save "test-colony" is loaded
    And game speed is paused
    And I close all dialogs
    And a colonist "Believer" exists

  Scenario: a believer reading their own faith's book grows steadier
    Given an ideoligion book for "Believer"'s own faith is spawned near "Believer"
    When "Believer" reads it for 400 ticks
    Then "Believer"'s certainty increased
    And no errors were logged

  Scenario: reading a rival faith's book erodes certainty
    Given an ideoligion book for a rival faith is spawned near "Believer"
    When "Believer" reads it for 400 ticks
    Then "Believer"'s certainty decreased
    And no errors were logged
