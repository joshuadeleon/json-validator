# Json Validation

A validator for string valued JSON ... and maybe more one day.

## Reasoning

1. Have you ever found yourself using a data store or API which only returns JSON 
data with string values regardless of the data type? 

2. Have you ever found this data changing out from under you? For example, what was 
a bool is now some kind of enum that only the DBA knows about because they didn't 
tell you they changed it.

3. Have you ever found out there are new fields in your data schema because no one 
bothered to communicate these new fields to you?

4. Have you ever found out there are missing required fields and you discovered it
because your app is breaking?

I have. Call it bad project management but this was the reality I found myself in.
In order to gain some sanity and metrics and begin a conversation about data contracts,
I know have this prototype. It might be useful for data stores which return JSON but 
only as string values due to their nature. You know, possibly HBase...

## Status

This is very much a prototype but has enough functionality to be useful at the moment.

There is a Test project and Console Project to help get you started on using the class libraries.
