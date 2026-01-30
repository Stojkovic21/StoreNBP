using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;

[ApiController]
[Route("relationship")]
public class CustomerBillRelationship : ControllerBase
{
    private readonly IDriver driver;
    private readonly Relationsip createRelationship;

    public CustomerBillRelationship()
    {
        var uri = Environment.GetEnvironmentVariable("URI");
        var user = Environment.GetEnvironmentVariable("Username");
        var password = Environment.GetEnvironmentVariable("Password");

        driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        createRelationship = new Relationsip(driver);
    }
    [HttpPost]
    [Route("customerbill/connect")]
    public async Task<ActionResult> ConncetRelationshiCustomerBillAsync([FromBody] RelationshipModel relationshipModel)
    {
        var query = @"
            MATCH (a:Customer {id: $sourceId})
            MATCH (b:Bill {id: $targetId})
            MERGE (a)-[r: HAS_BILL]->(b)
            RETURN a, r, b";
        var parameters = new Dictionary<string, object>
        {
            { "sourceId", relationshipModel.SourceId },
            { "targetId", relationshipModel.TargetId }
        };

        return await createRelationship.Relationship(query, parameters);
    }
    [HttpDelete]
    [Route("customer/breakup")]
    public async Task<ActionResult> BreakupRelationshipCustomerBillAsync([FromBody] RelationshipModel relationshipModel)
    {
        var query = @"
            MATCH (a:Customer {id: $sourceId})-[r: HAS_BILL]->(b:Bill {id: $targetId})
            DELETE r
            RETURN a, b";
        var parameters = new Dictionary<string, object>
        {
            { "sourceId", relationshipModel.SourceId },
            { "targetId", relationshipModel.TargetId }
        };

        return await createRelationship.Relationship(query, parameters);
    }
}