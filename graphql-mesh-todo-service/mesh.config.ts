import { defineConfig } from '@graphql-mesh/compose-cli'
import { loadOpenAPISubgraph } from '@omnigraph/openapi'

export const composeConfig = defineConfig({
  subgraphs: [
    {
      sourceHandler: loadOpenAPISubgraph('Todo: ASP.NET', {
        source: './todo.aspnet.openapi.json'
      })
    },
    {
      sourceHandler: loadOpenAPISubgraph('Todo: Express', {
        source: './todo.express.openapi.json'
      })
    }
  ]
})