using OrchardCore.Modules.Manifest;

[assembly: Module(
    Author = ManifestConstants.OrchardCoreTeam,
    Website = ManifestConstants.OrchardCoreWebsite,
    Version = ManifestConstants.OrchardCoreVersion
)]

[assembly: Feature(
    Name = "Azure SMS Provider",
    Description = "Provides an SMS service provider leveraging Azure Communication Services (ACS)",
    Category = "SMS"
)]
