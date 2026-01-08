using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;

[ApiController]
[Route("relationship")]
public class ItemBillRelationship : ControllerBase
{
    private readonly IDriver driver;
    private readonly Relationsip createRelationship;
    public ItemBillRelationship()
    {
        var uri = Environment.GetEnvironmentVariable("URI");
        var user = Environment.GetEnvironmentVariable("Username");
        var password = Environment.GetEnvironmentVariable("Password");

        driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        createRelationship = new Relationsip(driver);
    }

    [HttpPost]
    [Route("itembill/connect/{count}")]
    public async Task<ActionResult> ConnectRelationshipItemBillAsync([FromBody] RelationshipModel relationshipModel, int count)
    {
        var query = @"
            MATCH (a:Item {id: $sourceId})
            MATCH (b:Bill {id: $targetId})
            MERGE (b)-[r: ITEM_ON_BILL]->(a)
            SET r.Count= $count
            RETURN a, r, b";
        var parameters = new Dictionary<string, object>
        {
            { "sourceId", relationshipModel.SourceId },
            { "targetId", relationshipModel.TargetId },
            {"count",count}
        };

        return await createRelationship.Relationship(query, parameters);
    }
    [HttpDelete]
    [Route("category/breakup")]
    public async Task<ActionResult> BreakupRelationshipItemBillAsync([FromBody] RelationshipModel relationshipModel)
    {
        var query = @"
            MATCH (a:Item {id: $sourceId})-[r: ITEM_ON_BILL]->(b:Bill {id: $targetId})
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