# Example 12 - Logging SQL Server from OnboardTask

In this example below, we log from OnboardTask to SQL Server. The first step is to query the SQL server connection parameters (previously set with IoT Explorer). 
After that We send the data of a water meter (in this example a virtual meter) to the SQL database.

  ....

  string SQLConStr;
  string PeriodTableName;
  SQL SQLlogger = new SQL();
  
  public async void Run(IBackgroundTaskInstance taskInstance)
  {
          ...
          
          var sqlres = await SQLInfo.GetConnectionInfo();
          if (sqlres.Success == true)
          {
              SQLConStr = sqlres.SQLConnectionString;
              PeriodTableName = sqlres.SQLPeriodLogsTableName;
              SQLlogger.ConnectionString = SQLConStr;
          }

          ....
  }
  
  int waterounter = 0;  //virtual counter

  /// <summary>
  /// IoT Onboard Task
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  private async void OnboardTask(object sender, EventArgs e)
  {
      try
      {
          SQLTableInfo.Common.PeriodDataTableRow NewRow = new SQLTableInfo.Common.PeriodDataTableRow();

          NewRow.TimestampUTC = DateTime.UtcNow;
          NewRow.TimestampLT = DateTime.Now;
          NewRow.Namespace = "Test System";
          NewRow.SourceName = "Meter1";
          NewRow.Classification = "Water meter";
          NewRow.QuantityName = "Water";
          NewRow.QuantityTypeName = "Counter";
          NewRow.Unit = "m3";
          waterounter += 1;
          NewRow.Value = Convert.ToDouble(waterounter);
          NewRow.Calculated = true;

          var res = Period.Add(NewRow);

          if (Period.Count() >=10)
          {
              //Log into SQL database
              var sqlres = SQLlogger.SQLBulkInsert(PeriodTableName, Period.Get());
              if (sqlres.Success == true)
              {
                  //If logging success clear temporary table
                  Period.Clear();
              }
          }
      }
      catch (Exception ex)
      {
          EventLogging.AddLogMessage(MessageType.ExceptionError, this.GetType().Name + " - " + ServiceDisplayName + " - " + "OnboardTask exception error! Error: " + ex.Message);
      }
      OnboardTaskHandler.Run();  //Task continues to run
  }
  
  
