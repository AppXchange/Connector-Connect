namespace Connector.App.v1.Projects
{
    using Json.Schema.Generation;
    using System;
    using System.Text.Json.Serialization;
    using Xchange.Connector.SDK.CacheWriter;

    /// <summary>
    /// Data object that represents a project in the Xchange system. This will be converted to a JsonSchema, 
    /// so add attributes to the properties to provide any descriptions, titles, ranges, max, min, etc... 
    /// These types will be used for validation at runtime to ensure the objects being passed through the system 
    /// are properly formed. The schema also helps provide integrators more information for what the values 
    /// are intended to be.
    /// </summary>
    [PrimaryKey("id", nameof(Id))]
    [Description("Example description of the object.")]
    public class ProjectsDataObject
    {
        [JsonPropertyName("id")]
        [Description("Example primary key of the object.")]
        public Guid Id { get; init; }

        [JsonPropertyName("name")]
        [Description("The name of the project.")]
        [MaxLength(255)]
        public required string Name { get; set; }

        [JsonPropertyName("rootId")]
        [Description("The root ID of the project.")]
        public required string RootId { get; set; }

        [JsonPropertyName("thumbnail")]
        [Description("URL to the project's thumbnail image.")]
        public required string Thumbnail { get; set; }

        [JsonPropertyName("address")]
        [Description("Address of the project location.")]
        public required Address Address { get; set; }

        [JsonPropertyName("crs")]
        [Description("Coordinate Reference System.")]
        public required CoordinateReferenceSystem Crs { get; set; }

        [JsonPropertyName("location")]
        [Description("Location identifier.")]
        public required string Location { get; set; }

        [JsonPropertyName("lastVisitedOn")]
        [Description("Timestamp of the last visit.")]
        public DateTime LastVisitedOn { get; set; }

        [JsonPropertyName("modifiedOn")]
        [Description("Timestamp of the last modification.")]
        public DateTime ModifiedOn { get; set; }

        [JsonPropertyName("createdOn")]
        [Description("Timestamp of the creation.")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("updatedOn")]
        [Description("Timestamp of the last update.")]
        public DateTime UpdatedOn { get; set; }

        [JsonPropertyName("createdBy")]
        [Description("Information about the user who created the project.")]
        public required User CreatedBy { get; set; }

        [JsonPropertyName("modifiedBy")]
        [Description("Information about the user who last modified the project.")]
        public required User ModifiedBy { get; set; }

        [JsonPropertyName("size")]
        [Description("Size of the project.")]
        public long Size { get; set; }

        [JsonPropertyName("filesCount")]
        [Description("Number of files in the project.")]
        public int FilesCount { get; set; }

        [JsonPropertyName("foldersCount")]
        [Description("Number of folders in the project.")]
        public int FoldersCount { get; set; }

        [JsonPropertyName("versionsCount")]
        [Description("Number of file versions.")]
        public int VersionsCount { get; set; }

        [JsonPropertyName("usersCount")]
        [Description("Number of users with access.")]
        public int UsersCount { get; set; }

        [JsonPropertyName("description")]
        [Description("Description of the project.")]
        public required string Description { get; set; }

        [JsonPropertyName("startDate")]
        [Description("Start date of the project.")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        [Description("End date of the project.")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("license")]
        [Description("License details for the project.")]
        public required License License { get; set; }

        [JsonPropertyName("access")]
        [Description("Access level of the project.")]
        public required string Access { get; set; }
    }

    public class Address
    {
        public required string Text { get; set; }
        public required string Geometry { get; set; }
    }

    public class CoordinateReferenceSystem
    {
        public required string Name { get; set; }
        public required string Csib64 { get; set; }
    }

    public class User
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Status { get; set; }
    }

    public class License
    {
        public Guid Id { get; set; }
        public required string SubscriptionId { get; set; }
        public required string OrgName { get; set; }
        public required string OwnerId { get; set; }
        public required string EcomSource { get; set; }
        public required string Type { get; set; }
        public required string Sku { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int MaxProjects { get; set; }
        public int MaxStorage { get; set; }
        public int MaxInvites { get; set; }
        public int MaxCollaborators { get; set; }
        public int MaxUsers { get; set; }
        public int MaxForms { get; set; }
        public bool MRP { get; set; }
        public bool StatusShare { get; set; }
        public int UsedProjects { get; set; }
        public int UsedStorage { get; set; }
        public int UsedInvites { get; set; }
        public required string UpgradeLink { get; set; }
        public required string EcomAdminPortalUrl { get; set; }
    }
}
