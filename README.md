# Resource Janitor
This is a total rip-off of Jeff Hollan's great work for managing and alerting you to be able to manage your Azure Resources outside of existing Cost Management/Budget/Monitor capabilities.  Simply put, this puts quick controls at your fingertips for purging or retaining specific Azure Resources.

When you create a resource you'll get a text message with an index assigned to that resource.  You'll then be able to extend expiration or delete the resource via text message.

## Setup
This requires a few things to set up in current form:
- An Azure Subscription (obviously)
- A Twilio Account SID, Token, and Phone Number
- A system scope term definition for Event Grid
- An Azure Function deployment 
 
### Setup the Azure Function
Simple, create an Azure Functions resource that you'll be deploying the app to.  This deploy a Twilio binding and an Event Grid trigger.

You will need the Twilio Function URL for later in the Twilio setup.

### Setup the Scope term
Create a new Event Grid System Topic:
- Topic Type = Azure Subscriptions
- Subscription = Choose your subscription to monitor
- Resource group = whatever RG you want this topic to be in
- Name = Some name for your topic, e.g., 'resource-janitor-system-topic'

### Setup the Event Subscription on your Azure Subscription
In your subscription blade, go to Events and add a new Event Subscription.  Change the filter to events to only be `Resource Write Success` as the only option.  Your Endpoint will be pointing to your Azure Function and select the 'gridTrigger' Event Grid Trigger.

### Enable Azure Function as Contributor role 
In your subscription blade, go to IAM settings and add a role assignment for your Azure Funtion as Contributor.  This enables the function to actually manage the resource for cleanup.

#### Configurations
You need a few configuration variables on the Azure Funtion to work:
| Variable  | Notes  |
|---|---|---|---|---|
| TwilioSid | Your Account SID from Twilio |
| TwilioToken | Your Token from Twilio |
| TwilioFrom | The phone number from your Phone Messaging programmability |
| TwilioTo | The phone number to send the alerts to |
| RESERVED_PREFIXES | A semicolon delimited list of prefixes to ignore when resource groups are created.  Anything matching this is not monitored after creation. |

### Setup Twilio
In your Twilio account you want Programmable Messaging.  Navigate then to your Phone Number and the Messaging section and add a Webhook handler to your Function URL (with key) to the TwilioTrigger function endpoint.  This handles the back-and-forth conversation with the Janitor.