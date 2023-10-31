using ApolloGraphQL.HotChocolate.Federation;

namespace Products;

public class ProductResearch
{

    public ProductResearch(CaseStudy study, string? outcome)
    {
        Study = study;
        Outcome = outcome;
    }

    public CaseStudy Study { get; }
    public string? Outcome { get; }
}

public class ProductResearchType : ObjectType<ProductResearch>
{
    protected override void Configure(IObjectTypeDescriptor<ProductResearch> descriptor)
    {
        descriptor.BindFieldsImplicitly();
        descriptor.Field(pr => pr.Study).Type<NonNullType<CaseStudyType>>();
        descriptor.Key("study { caseNumber }")
            .ResolveReferenceWith(pr => GetProductReasearchByCaseNumber(default!, default!));
    }

    // TODO [Map]
    private static ProductResearch? GetProductReasearchByCaseNumber(
        [Map("study.caseNumber")] string caseNumber,
        Data repository)
    {
        return repository.ProductResearches.FirstOrDefault(
            r => r.Study.CaseNumber.Equals(caseNumber));
    }
}
