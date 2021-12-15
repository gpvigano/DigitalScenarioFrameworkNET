# Digital Scenario Framework for .NET

[Digital Scenario Framework] is a trial to study the collaboration between human and artificial intelligence (AI) applied to a generic digital scenario.

A wrapper was developed for the C++/STL framework library (`DiScenFw`) with a C-style programming interface that can be used in any platform, assuming suitable binaries for the library are provided.

This binding for .NET platform (`DiScenFwNET`) was developed upon that wrapper, mainly to test the framework also with [Unity](https://unity3d.com/unity), a popular game engine based on .NET/C# as scripting language.

**Please note that this binding is not yet complete** and can be not up to date with the original [Digital Scenario Framework] library (in particular it cannot exploit all its features until the wrapper is updated).

## Related projects

### Digital Scenario Framework

[Digital Scenario Framework], the project on which this work is based, is a trial to study the cooperation between human and artificial intelligence applied to a generic digital scenario.

###  Digital Scenario Framework Test for Unity

The [UnityDigitalScenarioTest] project, based on this project, is a test of [Digital Scenario Framework] interoperability with [Unity] game engine.

**Note:** When working with [UnityDigitalScenarioTest] it is possible in *DiScenFwNET* Visual Studio project to automatically update binaries in the [UnityDigitalScenarioTest] project folders: just set the variable `UNITY_TEST_ROOT` in *DiScenFwNET Properties|Build Events|Post-build event command line* to the path of Unity test project.

## Supported Platforms

This project was developed with Microsoft [Visual Studio] 2015 and is only tested with `DiScenFw` DLL for Windows platform at the moment.


## Licensing

The project is [licensed](/LICENSE.txt) under the [MIT license](https://opensource.org/licenses/MIT). This means you're free to modify the source and use the project in whatever way you want, as long as you attribute the original authors.

## Contact

Follow Digital Scenario Framework project updates on [GitHub](https://github.com/gpvigano/DigitalScenarioFramework).



[Visual Studio]: https://visualstudio.microsoft.com/
[Unity]: https://unity.com/

[Digital Scenario Framework]: https://github.com/gpvigano/DigitalScenarioFramework
[UnityDigitalScenarioTest]: https://github.com/gpvigano/UnityDigitalScenarioTest

