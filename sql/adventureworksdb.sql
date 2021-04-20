/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [ProductID]
      ,[Name]
      ,[ProductNumber]
      ,[MakeFlag]
      ,[FinishedGoodsFlag]
      ,[Color]
      ,[SafetyStockLevel]
      ,[ReorderPoint]
      ,[StandardCost]
      ,[ListPrice]
      ,[Size]
      ,[SizeUnitMeasureCode]
      ,[WeightUnitMeasureCode]
      ,[Weight]
      ,[DaysToManufacture]
      ,[ProductLine]
      ,[Class]
      ,[Style]
      ,[ProductSubcategoryID]
      ,[ProductModelID]
      ,[SellStartDate]
      ,[SellEndDate]
      ,[DiscontinuedDate]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2019].[Production].[Product]


  select [ProductID]
      ,[Name]
      ,[ProductNumber]
      ,[ProductSubcategoryID]
  from [AdventureWorks2019].[Production].[Product]
  where [ProductSubcategoryID] is not null
  order by [ProductSubcategoryID]

  SELECT product.[ProductID]
      ,product.[Name]
      ,product.[ProductNumber]
      ,product.[ProductSubcategoryID]
	  ,sub.[ProductSubcategoryID]
	  ,sub.[ProductCategoryID]

  FROM [AdventureWorks2019].[Production].[Product] AS product
  INNER JOIN [AdventureWorks2019].[Production].[ProductSubcategory] AS sub
  ON product.[ProductSubcategoryID] = sub.[ProductSubcategoryID]