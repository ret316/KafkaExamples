var builder = DistributedApplication.CreateBuilder(args);

//var messaging = builder.AddKafka("messaging")
//    .WithKafkaUI()
//    .WithExternalHttpEndpoints();


//var massTransitProducer = builder.AddProject<Projects.MassTransit_Producer>("masstransit-producer")
//    //.WithReference(messaging)

//    ;

//builder.AddProject<Projects.MassTransit_Consumer>("masstransit-consumer")
//    //.WithReference(messaging)
//    .WaitFor(massTransitProducer)
//    ;

//var confProd = builder.AddProject<Projects.ConfluentKafka_Producer>("confluentkafka-producer")
//    //.WithReference(messaging)
//    ;

//builder.AddProject<Projects.ConfluentKafka_Consumer>("confluentkafka-consumer")
//    //.WithReference(messaging)
//    .WaitFor(confProd)
//    ;

//builder.AddProject<Projects.NServiceBus_Producer>("nservicebus-producer")
//    //.WithReference(messaging)
//    ;

//builder.AddProject<Projects.NServiceBus_Consumer>("nservicebus-consumer")
//    //.WithReference(messaging)
//    ;

//var rebusProducer = builder.AddProject<Projects.Rebus_Producer>("rebus-producer")
//    //.WithReference(messaging)
//    ;

//builder.AddProject<Projects.Rebus_Consumer>("rebus-consumer")
//    //.WithReference(messaging)
//    .WaitFor(rebusProducer)
//    ;

/*builder.AddProject<Projects.CAP_Producer>("cap-producer");

builder.AddProject<Projects.CAP_Consumer>("cap-consumer");*/

builder.AddProject<Projects.Wolverine_Producer>("wolverine-producer");

builder.AddProject<Projects.Wolverine_Consumer>("wolverine-consumer");

builder.Build().Run();
