using System;

namespace UtilJsonApiSerializer
{
    public interface IPreSerializerPipelineModule
    {
        void Run(object objectData);
    }
}