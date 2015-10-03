# SwiftTailer
####A lightweight log tailer with some practical options

SwiftTailer is an open-source log tailer application that was put together to address the short-comings of other popular .Net log tailing projects as in terms of how *we* needed to use it at my current job. This tailer focuses on the idea of tailing multiple logs simultaneously and also saving pre-defined "group" or "sessions" which should prove useful in scenarios where you have many logs to monitor across different environments. For example, if you have 20 different logs that you want to monitor in 3 different environments, such as dev, staging, and production, then you can preconfigure these groups with all of the corresponding file locations so that you can quickly switch between them.

## Here are a couple goals with this project
  - Use modern practices and patterns as of .Net 4.5.1
  - Low CPU usage
  - Do not overwhelm the UI with options, try to get operations contextual and sematic
  - Keep the UI somewhat responsive
  - Do not overdo search options in the main view. Advanced searching and log parsing should be done somewhere other than the main view as not to overcomplicate the basic functionality.



