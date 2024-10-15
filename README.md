# VL.Devices.RPLidar

For using RPLIDAR devices by [Slamtec](http://slamtec.com).  
- List of potentially [supported devices](https://github.com/slamtec/rplidar_sdk?tab=readme-ov-file#supported-platforms)
- Tested to work with [RPLidar A1](https://www.slamtec.com/en/Lidar/A1) and [RPLidar A2](https://www.slamtec.com/en/Lidar/A2) model A2M12
- As of now defaults to the "standard" scan mode and does not support any of the "express" scan modes

For use with vvvv, the visual live-programming environment for .NET: http://visualprogramming.net

## Getting started
- When connecting via USB, install [CP210x USB to UART Bridge](https://www.silabs.com/documents/public/software/CP210x_Universal_Windows_Driver.zip) 
- Install as [described here](https://thegraybook.vvvv.org/reference/hde/managing-nugets.html) via commandline:

    `nuget install VL.Devices.RPLidar -pre`

- Usage examples and more information are included in the pack and can be found via the [Help Browser](https://thegraybook.vvvv.org/reference/hde/findinghelp.html)

## Contributing
- Report issues on [the vvvv forum](https://discourse.vvvv.org/c/vvvv-gamma/28)
- For custom development requests, please [get in touch](mailto:devvvvs@vvvv.org)
- When making a pull-request, please make sure to read the general [guidelines on contributing to vvvv libraries](https://thegraybook.vvvv.org/reference/extending/contributing.html)

## Credits
Based on [RPLidar4Net.IO](https://www.nuget.org/packages/RPLidar4Net.IO)

## Sponsoring
Development of this library was partially sponsored by:  
* Emanuele Foti
