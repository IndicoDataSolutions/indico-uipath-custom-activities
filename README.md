# indico-uipath-custom-activities

For development, make sure to have UiPath's Activity Creator extension installed. You can find more details at https://docs.uipath.com/integrations/docs/how-to-create-activities

To create a nuget build, right click on the Indico.RPAActivities.Activities.Design package in the solution explorer, and choose the Publish option.

Remember that all activities should have a child relationship to the IndicoScope activity. Make sure to set the constraint correctly in the child activity constructor.
