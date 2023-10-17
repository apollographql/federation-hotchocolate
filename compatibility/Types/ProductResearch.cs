using ApolloGraphQL.HotChocolate.Federation;

namespace Products;

[Key("study { caseNumber }")]
public class ProductResearch
{

    public ProductResearch(CaseStudy study, string? outcome)
    {
        Study = study;
        Outcome = outcome;
    }

    public CaseStudy Study { get; }
    public string? Outcome { get; }

    [ReferenceResolver]
    public static ProductResearch? GetProductReasearchByCaseNumber(
        [Map("study.caseNumber")] string caseNumber,
        Data repository)
    {
        return repository.ProductResearches.FirstOrDefault(
            r => r.Study.CaseNumber.Equals(caseNumber));
    }
}
