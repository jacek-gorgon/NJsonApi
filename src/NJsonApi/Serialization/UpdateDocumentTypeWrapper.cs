using System;

namespace NJsonApi.Serialization
{
    // TODO can we deprecate this now?
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