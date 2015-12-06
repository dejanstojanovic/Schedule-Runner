[![Build status](https://ci.appveyor.com/api/projects/status/ddkgbpg3pkpkphip?svg=true)](https://ci.appveyor.com/project/dejanstojanovic/schedulerunner)

# Schedule Runner
Managed scheduled code execution

##What does do?
The ScheduleRunner class instance picks up all assemblies which name starts with "Schedule." which are in "Schedules" folder and collects classes which implement ISchedule interface.
Loaded classes will be periodically checked for scheduled excecution time. If the time of the scheduled instance mathes current time, schedule instace code will be executed in separate thread.
