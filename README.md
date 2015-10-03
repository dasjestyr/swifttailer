# SwiftTailer
####A lightweight log tailer with some practical options

SwiftTailer is an open-source log tailer application that was put together to address some short-comings of other popular .Net log tailing projects in terms of how *we* needed to use it at my current job. We have been having so much success with this log monitor that we decided we should share it with the community. This is also my first real attempt at WPF/XAML so it also doubled as a learning project.

This tailer focuses on the idea of tailing multiple logs simultaneously and also saving pre-defined "group" or "sessions" which should prove useful in scenarios where you have many logs to monitor across different environments. For example, if you have 20 different logs that you want to monitor in 3 different environments, such as dev, staging, and production, then you can preconfigure these groups with all of the corresponding file locations so that you can quickly switch between them.

## Here are a couple goals with this project
  - Use modern practices and patterns as of .Net 4.5.1
  - Low CPU usage
  - Do not overwhelm the UI with options, try to get operations contextual and sematic
  - Keep the UI somewhat responsive
  - Do not overdo search options in the main view. Advanced searching and log parsing should be done somewhere other than the main view as not to overcomplicate the basic functionality.
  - Contributors should take their time in adding new functionality and keep clean, SOLID code that is easy to follow so that others may understand and contribute



