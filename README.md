# Resumo #

O sistema Web Crawler trata-se de uma prova de conceito(POC) e representa uma solução desenvolvida para automatizar a busca e extração de dados do portal Extrato Clube (Website o qual foi proposto para a POC), utilizando tecnologias de ponta e uma abordagem abrangente, o Web Crawler capacita a coleta eficiente e precisa de informações relevantes a partir deste portal.

### Back-end
Estrutura construída em torno da plataforma .NET Core, que oferece uma base sólida para desenvolvimento e escalabilidade, com foco principalmente em performance e também em simplicidade (por se tratar de uma POC).

- Web API com ASP .NET Core
- Mensageria com RabbitMQ
- Indexação e busca de dados com ElasticSearch
- Cache com Redis
- WebDriving com Selenium
- Testes unitários com XUnit
- Clean Architeture
- SOLID
- Repository Pattern
- CQRS

### Front-end
Estrutura desenvolvida em ReactJS, uma UI simples e minimalista, projetada exclusivamente para demonstração e facilitação do uso dos recursos do Web Crawler.

- React Context
- React Hooks
- Integração com API
- Information Toasts
- Loading Spinners
- Responsividade (Limitada a larguras de 520px -> 1920px seguindo os padrões de componentes BS)

# Índice #

- [Resumo](#resumo)
- [Introdução](#introdução)
  - [Sobre](#sobre)
  - [Funcionamento](#funcionamento)
- [Preparando o ambiente](#preparando-o-ambiente)
  - [Requisitos](#requisitos)
  - [Instalação](#instalacao)
  - [Configuração](#configuracao)
- [Uso do sistema](#uso-do-sistema)

# Introdução #

### Sobre ###

Antes de entender o funcionamento do sistema, é importante evidenciar qual foi a abordagem para construção da estrutura e o porque dessas decisões terem sido tomadas, aqui temos alguns tópicos com essas informações:

- O projeto Back-end do Web Crawler foi desenvolvido seguindo uma arquitetura monolítica, embora reconheça que essa abordagem possa apresentar limitações em termos de escalabilidade, optei por não adotar a arquitetura de microsserviços, utilizando de projetos externos como por exemplo workers para operações assíncronas com as filas. 
Essa decisão foi tomada por conta da demanda específica deste projeto, onde essa complexidade adicional não é necessária, por se tratar de uma prova de conceito, a ênfase recai mais sobre a simplicidade e eficácia do sistema, focando menos na demonstração de conceitos mais complexos quando não há a necessidade.

- O sistema não possui conexão com um banco de dados relacional, devido aos tipos de dados que devem ser armazenados nessa abordagem, uma conexão com um banco não foi necessária para realizar o trabalho de persistência, que foi realizado com cache em memória (Redis), arquivos de texto em determinadas abordagens e com o serviço do Elastic Search. Esssa abordagem também agrega na simplicidade do uso do sistema, sendo uma dependência a menos para ser considerada.

- Os testes unitários foram realizados com ênfase nos handlers e services, utilizando mocks e wrappers para as dependências externas e focando nos métodos que concentram o funcionamento da aplicação como todo.

- O sistema Front-end foi trabalhado da forma mais minimalista e performática possível, por se tratar de uma prova de conceito, o foco ficou na agilidade e clareza da interface de forma com que a experiência do usuário e a qualidade do sistema fossem mantidas.

### Funcionamento ###

O fluxo de funcionamento para o usuário da aplicação, começa a partir do Front-end, uma vez que já temos o Back-end devidamente configurado (Veja mais na seção [Preparando o ambiente](#preparando-o-ambiente)).

Ao executar o Crawler via interface do usuário, é enviada uma requisição HTTP para a API, no processamento dessa requisição temos o seguinte fluxo:

1 - Obter CPF's que devem ser pesquisados no Portal ExtratoClube. <br/>
2 - Enviar mensagem com os respectivos CPF's para uma fila (producer). <br/>
3 - Consumir mensagem da fila, obtendo os CPF's a serem pesquisados (consumer). <br/>
4 - Obter cache e filtrar a lista de CPF's (Redis), o CPF é removido da lista caso já exista no cache. <br/>
5 - Iniciar o WebDriving (Selenium), realizando a busca por cada um dos CPF's da lista filtrada. <br/>
6 - Obter os números de matrícula de cada CPF, que ao serem obtidos são indexados no Elastic Search. <br/>
7 - Incluir novos resultados no cache(Redis). <br/>
8 - Retornar o número de registros afetados. <br/>

Com isso, sempre que o Crawler é executado, é feita uma busca no back-end por novos CPF's no arquivo de dados (CPF's que ainda não estão no cache do Redis ou no index do ElasticSearch) para que seja feita a busca via WebDriving.<br/><br/>
Sabendo que o Crawler já foi executado, é possível realizar uma busca por CPF via interface do usuário, onde uma requisição HTTP é enviada para a API, que se comunica com o Elastic Search e retorna o resultado obtido via Crawler.

# Preparando o ambiente #


