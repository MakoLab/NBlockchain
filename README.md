# NBlockchain

This repository contains start-up implementation of the Blockchain node. First approach of this implementation has been inspired by the code [naivechain](https://github.com/lhartikk/naivechain).

The blockchain is recognized as a technology aimed at managing a cryptocurrency and solving the double spending problem. In principle the technology offers creation-to-end protection of financial transactions where a transaction is frozen after having been accepted by a community, i.e. it is made available by a peer-to-peer network in a context, which makes it read only over its entire life-cycle. It is designed to defeat any attempts at modifying because no party including the originator can change the data being maintained by the network of nodes.

It makes this technology a perfect option to publish and protect any irrevocable information, e.g. identifiers, financial reports, insurances, certificates, privileges, licenses, tax forms, etc.

This implementation addresses the requirement to use blockchain as universal data store. 

This project is a prototyping workspace to build a generic data store solution on top of the blockchain technology.
 
Description of the blockchain technology main technology features and how to use it to permanently store identifiers is covered by the article:

[IoT in the Context of Blockchain Main Technology Features](https://www.linkedin.com/pulse/iot-context-blockchain-main-technology-features-mariusz-postol/)

In this article the following main parts have been distinguished:

- `Network`: peer to peer (p2p) protocol implementation, a common peer-to-peer protocol that is to be used to assure interoperability of the node agents exposed by the Network Access Point (NAP) to the network;
- `AgentAPI`: Application Program Interface (API) to be exposed to an upper layer client application (proprietary user software); reading and modifying the data stored in the blocks. Functionality and implementation technology of the API depends on the proprietary requirements of the client application;
- `Repository`: local data chain store

To develope the application the following design decision has been made:

* Use Websockets to communicate with other nodes (P2P) and a super simple "protocols" in P2P communication
* No proof-of-work or proof-of-stake: a block can be added to the blockchain without competition
* API for upper layer application is implemented using the HTTP interface to control the node
* Data is not persisted in nodes

## Acknowledgement:

The work is realized  as part of a project co-funded by the National Center for Research and Development (POIR.01.01.01-00-0982/16).
