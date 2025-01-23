# StockQuoteAlert 📈

## Description

This is a challenge for Inoa interview process. The project consists in a stock quote alert system. You
choose an stock ticker, for example 'PETR4', and a maximum and minimum price. The goal of the system is to
send an email recommending you to buy or sell, always when the price of the stock goes above the maximum price, 
or under the minimum price.

## Technical aspects

For that project, I explored some desing patterns to facilitate the development of the system. For example,
I decided to split the Solution between 3 different projects: StockQuoteAlert, StockQuoteAlert.Models and 
StockQuoteAlert.Services. This turned the project more modular and easier to make changes and give maintenance.

The use of Dependency Injection improved the quality of the code and made it easier to modify by being more
loosely coupled. I can add a new service or replace a service that is already being used with much more flexibility.

I used Serilog to add some loggings in the 'logs' directory to help debug API and SMTP errors, 
also to track the requests and email sendings.

To get the price of the stocks I used the trial version of [brapi.dev](https://brapi.dev) and for the SMTP provider 
I used the SMTP Email Testing Inbox of [mailtrap.io](https://mailtrap.io). **OBS.:** The Brapi API updates stock prices from 30 to 30 minutes so I used this interval to check stock prices as well.

## How to run this project

1. Download the `release.zip` in the Releases section, and extract it. You can as well just `git clone` the project.
	- OBS.: If you choose to git clone the project, just open it in some .NET IDE and build the project
2. In the zip file as well as in the project itself, you will have an `appsettings.user.json` file that you will need to edit: 
    - 2.1. Brapi API Key - [brapi.dev](https://brapi.dev)
    - 2.2. Email Sender (Name and Email Address) - Who sends the email
    - 2.3. Email Receiver (Name and Email Address) - Who will receive the email
    - 2.4. SMTP setup - For this I used [mailtrap.io](https://mailtrap.io)

```json
{
    "BrapiApi": {
        "Key": "BRAPI-API-KEY"
    },
    "Email": {
        "Sender": {
            "Name": "EMAIL-SENDER-NAME",
            "Address": "EMAIL-SENDER-ADDRESS"
        },
        "Receiver": {
            "Name": "EMAIL-RECEIVER-NAME",
            "Address": "EMAIL-RECEIVER-ADDRESS"
        }
    },
    "Smtp": {
        "Host": "YOUR-SMTP-HOST",
        "Port": "YOUR-SMTP-PORT",
        "Username": "YOUR-SMTP-USERNAME",
        "Password": "YOUR-SMTP-PASSWORD"
    }
}
```

3. Run the project passing 3 parameters: the stock ticker, the sell price, and the buy price 
  - `.\stock-quote-alert.exe <TICKER> <SELL-PRICE> <BUY-PRICE> Ex.: .\stock-quote-alert.exe PETR4 22.67 22.59` 

## Packages used 
- MailKit
- MimeKit
- Newtonsoft.Json
- Serilog