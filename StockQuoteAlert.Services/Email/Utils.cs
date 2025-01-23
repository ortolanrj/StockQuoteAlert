namespace StockQuoteAlert.Services.Email;
public static class Utils
{
    public static String emailTemplate = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset=""utf-8"" />

        <style>
            body {{
                padding: 10px;
                display: flex;
                justify-content: space-around;
            }}

            .container {{
                max-width: 500px;
                max-height: 400px;
                background: #023562;
                color: white;
                font-family: Arial;
                padding: 20px;
                border-radius: 5px;
                margin-top: 50px;
            }}
        </style>
    </head>
    <body>
        <div class=""container"">
            {{mailMessage}}
            <img src=""https://www.inoa.com.br/images/logo-bottom.png"" />
        </div>
    </body>
    </html>";
}

