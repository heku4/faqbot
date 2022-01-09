### FAQBot

Telegrm-bot **FAQBot** quickly answers questions. 
Questions and answers to them are stored in the file `src/faq.json`.
The application uses the [Telegram-bot](https://github.com/TelegramBots) client.

#### faq.json scheme
```
{
  "type": "array",
  "items": [
    {
      "type": "object",
      "properties": {
        "Question": {
          "type": "string"
        },
        "Answer": {
          "type": "object",
          "properties": {
            "Text": {
              "type": "string"
            },
            "Type": {
              "type": "integer"
            }
          },
          "required": [
            "Text",
            "Type"
          ]
        }
      },
      "required": [
        "Question",
        "Answer"
      ]
    },
    {
      "type": "object",
      "properties": {
        "Question": {
          "type": "string"
        },
        "Answer": {
          "type": "object",
          "properties": {
            "VenueData": {
              "type": "object",
              "properties": {
                "Latitude": {
                  "type": "number"
                },
                "Longitude": {
                  "type": "number"
                },
                "Address": {
                  "type": "string"
                },
                "Title": {
                  "type": "string"
                }
              },
              "required": [
                "Latitude",
                "Longitude",
                "Address",
                "Title"
              ]
            },
            "Type": {
              "type": "integer"
            }
          },
          "required": [
            "VenueData",
            "Type"
          ]
        }
      },
      "required": [
        "Question",
        "Answer"
      ]
    },
    {
      "type": "object",
      "properties": {
        "Question": {
          "type": "string"
        },
        "Answer": {
          "type": "object",
          "properties": {
            "LocationData": {
              "type": "object",
              "properties": {
                "Latitude": {
                  "type": "number"
                },
                "Longitude": {
                  "type": "number"
                }
              },
              "required": [
                "Latitude",
                "Longitude"
              ]
            },
            "Type": {
              "type": "integer"
            }
          },
          "required": [
            "LocationData",
            "Type"
          ]
        }
      },
      "required": [
        "Question",
        "Answer"
      ]
    },
    {
      "type": "object",
      "properties": {
        "Question": {
          "type": "string"
        },
        "Answer": {
          "type": "object",
          "properties": {
            "DocumentData": {
              "type": "object",
              "properties": {
                "DocumentUrl": {
                  "type": "string"
                }
              },
              "required": [
                "DocumentUrl"
              ]
            },
            "Type": {
              "type": "integer"
            }
          },
          "required": [
            "DocumentData",
            "Type"
          ]
        }
      },
      "required": [
        "Question",
        "Answer"
      ]
    },
    {
      "type": "object",
      "properties": {
        "Question": {
          "type": "string"
        },
        "Answer": {
          "type": "object",
          "properties": {
            "ContactData": {
              "type": "object",
              "properties": {
                "FirstName": {
                  "type": "string"
                },
                "LastName": {
                  "type": "string"
                },
                "PhoneNumber": {
                  "type": "string"
                }
              },
              "required": [
                "FirstName",
                "LastName",
                "PhoneNumber"
              ]
            },
            "Type": {
              "type": "integer"
            }
          },
          "required": [
            "ContactData",
            "Type"
          ]
        }
      },
      "required": [
        "Question",
        "Answer"
      ]
    }
  ]
}
```
