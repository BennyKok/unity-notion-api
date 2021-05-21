# Notion API for Unity

This is very initial draft of non-official Notion Beta API Client for Unity

## Aim

- Zero dependencies
- Optional Json parser
- Focus on Database related endpoint
- Using Unity WebRequest + JsonUtility, basically should work on all platform 

## Example

Creating a NotionAPI object.

```csharp
var api = new NotionAPI(apiKey);
```

Example db scenario

### Card Database

| Name (Title)  | Tags (Multi-select) |
| ------------- | :-----------------: |
| Basic Attack  |   Attack, Defense   |
| Ranged Attack |       Defense       |

Getting the database object and querying the database as JSON.

```csharp
private IEnumerator Start()
{
    var api = new NotionAPI(apiKey);

    yield return api.GetDatabase<CardDatabaseProperties>(database_id, (db) =>
    {
        Debug.Log(db.id);
        Debug.Log(db.created_time);
        Debug.Log(db.title.First().text.content);
    });

    yield return api.QueryDatabaseJSON(database_id, (db) =>
    {
        Debug.Log(db);
    });
}

// For type parsing the db Property with JsonUtility
[Serializable]
public class CardDatabaseProperties
{
    public MultiSelectProperty Tags;
    public TitleProperty Name;
}
```

## TO-DO

- [x] Implement basic data model for Page
- [x] Type parsed method for QueryDatabaseJSON
- [ ] Data Model accessibility (Implicit operator + Date conversion)
- [ ] Filter options for db query?