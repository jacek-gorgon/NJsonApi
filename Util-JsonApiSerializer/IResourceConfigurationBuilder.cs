﻿namespace UtilJsonApiSerializer
{
    public interface IResourceConfigurationBuilder
    {
        IResourceMapping ConstructedMetadata { get; set; }
        IPreSerializerPipelineModule PreSerializerPipelineModule { get; set; }
    }
}