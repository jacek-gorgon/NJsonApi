using System;

namespace UtilJsonApiSerializer.Serialization
{
    public class UpdateDocumentTypeWrapper
    {
        public UpdateDocument UpdateDocument { get; private set; }
        public Type Type { get; private set; }

        public UpdateDocumentTypeWrapper(UpdateDocument updateDocument, Type type)
        {
            UpdateDocument = updateDocument;
            Type = type;
        }
    }
}