# ClassIdNet

ClassIdNet is a library for managing and manipulating directed graphs. It supports serialization and deserialization, as well as merging and comparing graphs.

## Installation

To install ClassIdNet, clone the repository and build the project using Visual Studio or .NET CLI.

```sh
git clone https://github.com/yourusername/ClassIdNet.git
cd ClassIdNet
dotnet build

Usage
Creating and Manipulating Graphs

using ClassIdNet;

// Create a new DirectedGraph
DirectedGraph graph = new DirectedGraph();

// Add nodes to the graph
Node node1 = new Node("Class1", "Node1", graph);
Node node2 = new Node("Class1", "Node2", graph);
graph.AddNode(node1);
graph.AddNode(node2);

// Add a link between nodes
node1.AddLinkTo("Class1", "Node2");

// Serialize the graph
List<string> serializedGraph = Serialization.SerializeGraph(graph);

// Deserialize the graph
DirectedGraph deserializedGraph = Serialization.DeserializeGraph(serializedGraph);


Merging Graphs

// Merge two graphs
graph.Merge(anotherGraph);


Changelog

[0.5.0] - 2024-06-10

    Added node merging functionality.
    Enhanced serialization and deserialization.
    Improved performance and fixed bugs.


License

This project is licensed under the MIT License.




