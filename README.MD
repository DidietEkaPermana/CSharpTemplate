# CSharp Template

This is a project template for C#. With a sample of 2 models which is Hotels and Rooms, in which Hotels contain Rooms.

## Project Structure
```
.
└── API
	|
	├── Core (Business Process)
	|
	├── Domain (Models)
	|
	└── Infrastructure (DB Connection, Messaging etc)
```
## API

API will contain REST declaration of project, Security validation and logging declaration. 
In controller, you should extract user name for audit from security token.
logging would be using serilog and in this sample will be consume in elasticsearch.

## Core

Core is the business process that you want to do and the place where do you want declare what technology that you want to use.
StartupSetup is the class declaration which technology that you want to use.
For implementation, there will be no different except if you want to use ElasticSearch as your persistent DB, because it has different style to use rather than LINQ.

## Domain

This is where you put your models, BaseModel would be use for MongoDB and ElasticSearch for ID in BaseRepository.

## Infrastructure

In this template, we show how to implement several technologies that we can implement without tightly coupled between the Core and Domain.
If you don't use, feel free to remove it.
There are 3 main section:

### 1. DB (Database Section)

Where you declare your Context, UnitOfWork and Repositories.
The Models attribute should be declare in your context, not in your Domain.
In this sample we can use MongoDB (4.2.3), CosmosDB (Emulator), SQLServer (14.0.3038.14 (X64) Dev Edition on Linux) and ElasticSearch (7.6.0).

### 2. Messaging

This is where you declare your messaging technology, in this template there are 2 technology sample that you can use, which is Kafka (zookeeper:3.4.9 and confluentinc/cp-kafka:5.0.1) and NATS (nats-streaming:0.17.0).

### 3. Storage

This section is where do you want to upload your file to.
For this template, you can upload to your file system or Azure Storage (Emulator)

