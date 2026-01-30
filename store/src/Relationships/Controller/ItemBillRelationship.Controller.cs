using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;

[ApiController]
[Route("relationship")]
public class ProductBillRelationship : ControllerBase
{
    private readonly IDriver driver;
    private readonly Relationsip createRelationship;
    public ProductBillRelationship()
    {
        var uri = Environment.GetEnvironmentVariable("URI");
        var user = Environment.GetEnvironmentVariable("Username");
        var password = Environment.GetEnvironmentVariable("Password");

        driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        createRelationship = new Relationsip(driver);
    }

    [HttpPost]
    [Route("productbill/connect")]
    public async Task<ActionResult> ConnectRelationshipProductBillAsync([FromBody] RelationshipModel relationshipModel)
    {
        var query = @"
            MATCH (a:Product {id: $sourceId})
            MATCH (b:Bill {id: $targetId})
            MERGE (b)-[r: PRODUCT_ON_BILL]->(a)
            SET r.Count= $count
            RETURN a, r, b";
        var parameters = new Dictionary<string, object>
        {
            { "sourceId", relationshipModel.SourceId },
            { "targetId", relationshipModel.TargetId },
            { "count",relationshipModel.Count}
        };

        return await createRelationship.Relationship(query, parameters);
    }
    [HttpDelete]
    [Route("category/breakup")]
    public async Task<ActionResult> BreakupRelationshipProductBillAsync([FromBody] RelationshipModel relationshipModel)
    {
        var query = @"
            MATCH (a:Product {id: $sourceId})-[r: PRODUCT_ON_BILL]->(b:Bill {id: $targetId})
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