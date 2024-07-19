namespace Belajar2.GraphQL.Cars
{
    public class CarInputType : InputObjectType<Car>
    {
        protected override void Configure(IInputObjectTypeDescriptor<Car> descriptor)
        {
            descriptor.Field(c => c.CarPurchases).Ignore();
        }
    }

}
