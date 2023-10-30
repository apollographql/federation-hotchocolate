namespace Products;

public class CaseStudy
{

    public CaseStudy(string caseNumber, string? description)
    {
        CaseNumber = caseNumber;
        Description = description;
    }

    public string CaseNumber { get; }
    public string? Description { get; }

}

public class CaseStudyType : ObjectType<CaseStudy>
{
    protected override void Configure(IObjectTypeDescriptor<CaseStudy> descriptor)
    {
        descriptor.BindFieldsImplicitly();
        descriptor.Field(f => f.CaseNumber).Type<NonNullType<IdType>>();
    }
}
