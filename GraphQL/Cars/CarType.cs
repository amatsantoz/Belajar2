namespace Belajar2.GraphQL.Cars
{
    public class CarType : ObjectType<Car>
    {
        protected override void Configure(IObjectTypeDescriptor<Car> descriptor)
        {
            descriptor.Field(c => c.Id).Type<NonNullType<IdType>>();
            descriptor.Field(c => c.Merk).Type<NonNullType<StringType>>();
            descriptor.Field(c => c.Model).Type<NonNullType<StringType>>();
            descriptor.Field(c => c.Year).Type<NonNullType<IntType>>();
            descriptor.Field(c => c.Stock).Type<NonNullType<IntType>>();
            descriptor.Field(c => c.CarPurchases).Ignore();
        }
    }
}
