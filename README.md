# HWMonitor
Repository for Android application "HW Monitor". Application is used to track and visualize the performance, usage and temperature of the hardware components inside the PC. 

### Reading of Hardware information and performance
This part of the service uses OpenHardwareMonitor (https://github.com/openhardwaremonitor/) project to get the information needed about the hardware components.

### Communication
Communication between the Android phone and PC can be via USB cable or via WiFi (the devices need to be on the same network). Communication part is done through .NET service over UDP protocol or serial communication. On first usage, user can select to search for the available devices on the network, or the USB connection will be automatically recognized. Network part is done using multicasting, where both apps know what endpoints to look for.

### Visualizations
Visualizations for the hardware components are theme-based and are customizable and expandable. Custom templates can be created to add a personal touch to the app. There are 3 predefined themes included, together with a sample for creating your own theme.

### Performance logging
The overall performance of individual components (can be selected) can be logged separately and exported. 
There are 2 types of logs:
* Visual log
* Text log

Both can be selected (or deselected) and customized. 
#### Visual log
This type of log provides a web page log that shows the performance over time using Google charts (https://developers.google.com/chart).

#### Text log
This type of log is a standard text log that shows the performance over time in a simple text file.

