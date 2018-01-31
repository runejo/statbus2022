using System.ComponentModel.DataAnnotations;
using System.Reflection;
using nscreg.Data.Entities;
using nscreg.ModelGeneration.PropertiesMetadata;
using nscreg.ModelGeneration.Validation;

namespace nscreg.ModelGeneration.PropertyCreators
{
    /// <summary>
    ///     Класс создатель свойства адреса
    /// </summary>
    public class AddressPropertyCreator : PropertyCreatorBase
    {
        public AddressPropertyCreator(IValidationEndpointProvider validationEndpointProvider) : base(
            validationEndpointProvider)
        {
        }

        public override bool CanCreate(PropertyInfo propInfo)
        {
            return propInfo.PropertyType == typeof(Address);
        }

        /// <summary>
        ///     Метод создатель свойства адреса
        /// </summary>
        public override PropertyMetadataBase Create(PropertyInfo propInfo, object obj, bool writable,
            bool mandatory = false)
        {
            return new AddressPropertyMetadata(
                propInfo.Name,
                mandatory,
                obj == null ? new Address() : (Address) propInfo.GetValue(obj),
                propInfo.GetCustomAttribute<DisplayAttribute>()?.GroupName,
                writable: writable
            );
        }
    }
}
