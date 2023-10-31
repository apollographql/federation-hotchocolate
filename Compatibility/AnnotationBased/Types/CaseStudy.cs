namespace Products;

public class CaseStudy
{

    public CaseStudy(string caseNumber, string? description)
    {
        CaseNumber = caseNumber;
        Description = description;
    }

    [GraphQLType(typeof(IdType))]
    [GraphQLNonNullType]
    public string CaseNumber { get; }
    public string? Description { get; }

}
