import { defineConfig } from '@graphql-mesh/compose-cli'
import { loadOpenAPISubgraph } from '@omnigraph/openapi'

export const composeConfig = defineConfig({
  subgraphs: [
    {
      sourceHandler: loadOpenAPISubgraph('Todo: ASP.NET', {
        source: './todo-rest-api-asp-net/todo.aspnet.openapi.json'
      })
    },
    {
      sourceHandler: loadOpenAPISubgraph('Todo: Express', {
        source: './todo-rest-api-express/todo.express.openapi.json'
      })
    }
  ]
})