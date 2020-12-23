# DbDiagram.Io To C#

converts scripts from https://dbdiagram.io to C# objects

### Known Issues
- can't process indexes
  - must be commented out for now
- works with `Ref: t1.f1 < t2.f2`. Doesn't work with `Ref: t1.f1 > t2.f2`

### Examples

**Input**

```
Table Category as Ctg {
  CategoryId int [pk, increment] // auto-increment
  // this line should be skipped
  Name varchar
  CreatedDateTime datetime
  CreatedBy varchar // username
  LastUpdatedDateTime datetime
  LastUpdatedBy varchar // username
  IsActive bit
}


Table FAQ {
  FAQId int [pk, increment]
  CategoryId int
  Question varchar
  Answer varchar
  CreatedDateTime datetime
  CreatedBy varchar // username
  LastUpdatedDateTime datetime
  LastUpdatedBy varchar // username
  RankClickCount bigint // count used for ranking - can get updated by ranking
  HistoricalClickCount bigint // count of click
  IsActive bit

//  Indexes {
//    (CategoryId) [name:'IX_CategoryId']
//  }
}


Ref: "FAQ"."CategoryId" < "Category"."CategoryId"

```

**Output**

```csharp
public class Category {
  public int CategoryId { get; set; }
  public string Name { get; set; }
  public DateTime CreatedDateTime { get; set; }
  public string CreatedBy { get; set; }
  public DateTime LastUpdatedDateTime { get; set; }
  public string LastUpdatedBy { get; set; }
  public bool IsActive { get; set; }
}

public class FAQ {
  public int FAQId { get; set; }
  public int CategoryId { get; set; }
  public Category Category { get; set; }
  public string Question { get; set; }
  public string Answer { get; set; }
  public DateTime CreatedDateTime { get; set; }
  public string CreatedBy { get; set; }
  public DateTime LastUpdatedDateTime { get; set; }
  public string LastUpdatedBy { get; set; }
  public long RankClickCount { get; set; }
  public long HistoricalClickCount { get; set; }
  public bool IsActive { get; set; }
}
```
