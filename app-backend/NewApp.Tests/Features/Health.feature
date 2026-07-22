Feature: Health endpoint
  As a monitoring system
  I want to query the ApiHost service health
  So that I can verify the service is running

  Scenario: Health endpoint reports that the service is healthy
    When I send a GET request to "/health"
    Then the response status code is 200
    And the reported health status is "Healthy"
