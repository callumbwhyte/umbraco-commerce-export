# Umbraco Commerce Export

<img src="docs/img/logo.png?raw=true" alt="Umbraco Commerce Export" width="250" align="right" />

[![NuGet release](https://img.shields.io/nuget/v/Umbraco.Commerce.Contrib.Export.svg)](https://www.nuget.org/packages/Umbraco.Commerce.Contrib.Export/)

## Getting started

This package is supported on Umbraco v10-v13 (LTS) and the corresponding versions of [Umbraco Commerce](https://umbraco.com/products/add-ons/commerce/).

### Installation

Umbraco Commerce Export is available via [NuGet](https://www.nuget.org/packages/Umbraco.Commerce.Contrib.Export/).

To install with the .NET CLI, run the following command:

    $ dotnet add package Umbraco.Commerce.Contrib.Export

To install from within Visual Studio, use the NuGet Package Manager UI or run the following command:

    PM> Install-Package Umbraco.Commerce.Contrib.Export

## Usage

Umbraco Commerce Export introduces a new `ExportRenderingNotification` that can be subscribed to in order to customise the Export Template rendering process.

By default Export Templates save the string output of a defined Razor view to disk. The contents of the Razor view can be [customised](https://docs.umbraco.com/umbraco-commerce/how-to-guides/customizing-templates) to output HTML or a CSV, but is otherwise limited and cannot return other binary formats like PDF.

The `ExportRenderingNotification` is broadcast when an Export Template is rendered, before it is downloaded to the browser. This allows the content (string) to be further processed and returned as a `Stream` containing any binary data desired.

A [notification handler](https://docs.umbraco.com/umbraco-commerce/key-concepts/events#notification-events) can be created like the below sample, and [registered via the `IUmbracoCommerceBuilder`](https://docs.umbraco.com/umbraco-commerce/key-concepts/events#registering-a-notification-event-handler).

```
public class ExportRenderingHandler : NotificationEventHandlerBase<ExportRenderingNotification>
{
    public override void Handle(ExportRenderingNotification evt)
    {
        // create a stream from the string content
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(evt.Content));

        // apply the stream to output
        evt.SetStream(stream);
    }
}
```

## Contribution guidelines

To raise a new bug, create an issue on the GitHub repository. To fix a bug or add new features, fork the repository and send a pull request with your changes. Feel free to add ideas to the repository's issues list if you would to discuss anything related to the library.

### Who do I talk to?

This project is maintained by [Callum Whyte](https://callumwhyte.com/) and contributors. If you have any questions about the project please get in touch on [Twitter](https://twitter.com/callumbwhyte), or by raising an issue on GitHub.

## Credits

The package uses the [Umbraco Commerce](https://umbraco.com/products/add-ons/commerce/) logo, which is the sole property of Umbraco A/S.

## License

Copyright &copy; 2025 [Callum Whyte](https://callumwhyte.com/), and other contributors

Licensed under the [MIT License](LICENSE.md).