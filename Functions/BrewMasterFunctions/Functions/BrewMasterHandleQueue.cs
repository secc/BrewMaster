using System;
using System.Threading.Tasks;
using BrewMasterFunctions.Contracts;
using BrewMasterFunctions.Data;
using BrewMasterFunctions.Model;
using BrewMasterFunctions.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using EmissaryClient;

namespace BrewMasterFunctions.Functions
{
    public class BrewMasterFunctions
    {
        private readonly IContainerFactory _containerFactory;

        public BrewMasterFunctions( IContainerFactory containerFactory )
        {
            _containerFactory = containerFactory;
        }

        [FunctionName( "BrewMasterHandleQueue" )]
        public async Task Run(
            [ServiceBusTrigger( "brewevent", Connection = "BrewMasterServiceBusConnectionString" )] string message,
            ILogger log
            )
        {
            BrewMasterEvent brewMasterEvent;

            brewMasterEvent = JsonSerializer.Deserialize<BrewMasterEvent>( message );


            if ( brewMasterEvent == null )
            {
                return;
            }

            switch ( brewMasterEvent.BrewMasterEventType )
            {
                case BrewMasterEventType.NoEvent:
                    await HandleNoEvent( brewMasterEvent );
                    break;
                case BrewMasterEventType.BrewStart:
                    await HandleBrewStart( brewMasterEvent );
                    break;
                case BrewMasterEventType.BrewComplete:
                    await HandleBrewComplete( brewMasterEvent );
                    break;
            }
        }

        private async Task HandleBrewComplete( BrewMasterEvent brewMasterEvent )
        {
            await EnsureCoffeeMakerExists( brewMasterEvent );
            await RecordBrewEvent( brewMasterEvent );
            await NotifyCompleteBrew( brewMasterEvent );
        }

        private async Task HandleBrewStart( BrewMasterEvent brewMasterEvent )
        {
            await EnsureCoffeeMakerExists( brewMasterEvent );
            await RecordBrewEvent( brewMasterEvent );

        }

        private async Task HandleNoEvent( BrewMasterEvent brewMasterEvent )
        {
            await EnsureCoffeeMakerExists( brewMasterEvent );
        }

        private async Task RecordBrewEvent( BrewMasterEvent brewMasterEvent )
        {
            var brewEventService = _containerFactory.GetService<BrewEvent>();
            var brewEvent = new BrewEvent
            {
                Id = Guid.NewGuid().ToString(),
                BrewMasterEventType = brewMasterEvent.BrewMasterEventType,
                CoffeeMakerId = brewMasterEvent.DeviceId,
                CompleteDateTime = brewMasterEvent.CompleteDateTime,
                StartDateTime = brewMasterEvent.StartDateTime
            };
            await brewEventService.AddAsync( brewEvent, brewEvent.Id );
        }

        private async Task EnsureCoffeeMakerExists( BrewMasterEvent brewMasterEvent )
        {
            var coffeeMakerService = _containerFactory.GetService<CoffeeMaker>();

            var coffeeMaker = await coffeeMakerService.GetAsync( brewMasterEvent.DeviceId );

            if ( coffeeMaker == null )
            {
                coffeeMaker = new CoffeeMaker
                {
                    Id = brewMasterEvent.DeviceId,
                    LastCompeteDateTime = brewMasterEvent.CompleteDateTime,
                    LastStartDateTime = brewMasterEvent.StartDateTime
                };

                await coffeeMakerService.AddAsync( coffeeMaker, coffeeMaker.Id );
            }
            else
            {
                coffeeMaker.LastStartDateTime = brewMasterEvent.StartDateTime;
                coffeeMaker.LastCompeteDateTime = brewMasterEvent.CompleteDateTime;

                await coffeeMakerService.UpdateAsync( coffeeMaker.Id, coffeeMaker );
            }

        }

        private async Task NotifyCompleteBrew( BrewMasterEvent brewMasterEvent )
        {
            var subscriptionService = _containerFactory.GetService<Subscription>();
            var coffeeMakerService = _containerFactory.GetService<CoffeeMaker>();

            var coffeeMaker = await coffeeMakerService.GetAsync( brewMasterEvent.DeviceId );
            var coffeeMakerId = brewMasterEvent.DeviceId;


            var subscriptions = await subscriptionService.GetAllAsync( $"SELECT * FROM c WHERE c.coffeeMakerId='{coffeeMakerId}'" );

            var emissaryClient = new MessageClient( new ClientSettings
            {
                SharedAccessKey = Environment.GetEnvironmentVariable( "SharedAccessKey" ),
                SharedSecretKey = Environment.GetEnvironmentVariable( "SharedSecretKey" )
            } );


            var email = @"<!DOCTYPE html>

<html lang=""en"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:v=""urn:schemas-microsoft-com:vml"">
<head>
<title></title>
<meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type""/>
<meta content=""width=device-width, initial-scale=1.0"" name=""viewport""/>
<!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml><![endif]-->
<style>
		* {{
			box-sizing: border-box;
		}}

		body {{
			margin: 0;
			padding: 0;
		}}

		a[x-apple-data-detectors] {{
			color: inherit !important;
			text-decoration: inherit !important;
		}}

		#MessageViewBody a {{
			color: inherit;
			text-decoration: none;
		}}

		p {{
			line-height: inherit
		}}

		@media (max-width:520px) {{
			.icons-inner {{
				text-align: center;
			}}

			.icons-inner td {{
				margin: 0 auto;
			}}

			.fullMobileWidth,
			.row-content {{
				width: 100% !important;
			}}

			.image_block img.big {{
				width: auto !important;
			}}

			.column .border {{
				display: none;
			}}

			table {{
				table-layout: fixed !important;
			}}

			.stack .column {{
				width: 100%;
				display: block;
			}}
		}}
	</style>
</head>
<body style=""background-color: #FFFFFF; margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""nl-container"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #FFFFFF;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-1"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row-content stack"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 500px;"" width=""500"">
<tbody>
<tr>
<td class=""column column-1"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"" width=""100%"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""heading_block"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td style=""width:100%;text-align:center;"">
<h1 style=""margin: 0; color: #555555; font-size: 23px; font-family: Arial, Helvetica Neue, Helvetica, sans-serif; line-height: 120%; text-align: center; direction: ltr; font-weight: 700; letter-spacing: normal; margin-top: 0; margin-bottom: 0;""><span class=""tinyMce-placeholder"">IT'S COFFEE TIME</span></h1>
</td>
</tr>
</table>
<table border=""0"" cellpadding=""10"" cellspacing=""0"" class=""paragraph_block"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"" width=""100%"">
<tr>
<td>
<div style=""color:#000000;font-size:14px;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-weight:400;line-height:120%;text-align:center;direction:ltr;letter-spacing:0px;"">
<p style=""margin: 0;"">{0} just finished brewing.</p>
</div>
</td>
</tr>
</table>
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""image_block"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td style=""width:100%;padding-right:0px;padding-left:0px;"">
<div align=""center"" style=""line-height:10px""><a href=""https://rock.secc.org/brewmaster"" style=""outline:none"" tabindex=""-1"" target=""_blank""><img alt=""It's a sloth"" class=""fullMobileWidth big"" src=""https://www.southeastchristian.org/Content/Apps/slothcoffee.png"" style=""display: block; height: auto; border: 0; width: 400px; max-width: 100%;"" title=""It's a sloth"" width=""400""/></a></div>
</td>
</tr>
</table>
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-2"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row-content stack"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 500px;"" width=""500"">
<tbody>
<tr>
<td class=""column column-1"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"" width=""100%"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""icons_block"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td style=""vertical-align: middle; color: #9d9d9d; font-family: inherit; font-size: 15px; padding-bottom: 5px; padding-top: 5px; text-align: center;"">
<table cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td style=""vertical-align: middle; text-align: center;"">
<!--[if vml]><table align=""left"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""display:inline-block;padding-left:0px;padding-right:0px;mso-table-lspace: 0pt;mso-table-rspace: 0pt;""><![endif]-->
<!--[if !vml]><!-->
<table cellpadding=""0"" cellspacing=""0"" class=""icons-inner"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block; margin-right: -4px; padding-left: 0px; padding-right: 0px;"">
<!--<![endif]-->

</table>
</td>
</tr>
</table>
</td>
</tr>
</table>
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table><!-- End -->
</body>
</html>";



            foreach ( var subscription in subscriptions )
            {
                await emissaryClient.SendEmailMessage( "BrewMaster", "brewmaster@secc.org", subscription.PersonAliasId, "Coffee Time", string.Format( email, coffeeMaker.Name ) );
            }

        }
    }
}