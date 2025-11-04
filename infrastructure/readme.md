## Creating Federated Credentials 

You can create a Federated Credential into Azure, so the GitHub runners can log into your Azure subscription and deploy resources. The Federated Credential helps to remove the need to use secrets. You won't need to swap app registration secrets around when they expire.

### Federated Credential Execution
You will need the following to execute this:
 - The Application Id (Client Id) of an Entra Application Registration to use for deployment.
 - The owner's name of your GH repository
 - The name of your repo "ProjectA.Web", not "https://myGHhanle/ProjectA.Web/"
 - The branch name you want to pull from.

First create and save this JSON information into a .json file to reference.
{
  "name": "github-actions",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:<owner>/<repo>:ref:refs/heads/<branch>>",
  "description": "OIDC for GitHub Actions",
  "audiences": ["api://AzureADTokenExchange"]
}

Then execute this command to build the Federated Credential in your Azure subscription.
az ad app federated-credential create --id <appId> --parameters @federated-credential-function.json

After that step is complete, you need to create three GitHub Actions secrets, which you will reference in your workflow yml.
1. AZURE_CLIENT_ID
2. AZURE_TENANT_ID
3. AZURE_SUBSCRIPTION_ID

Finally, when you create the workflow action, the Azure login step will need to have the following in the beginning of the workflow file to allow OIDC login
# Required for OIDC authentication
permissions:
  id-token: write
  contents: read

Then when you are ready to login to Azure, this would be the step you use.
- name: Azure Login (OIDC)
  uses: azure/login@v2
  with:
      client-id: ${{ secrets.AZURE_CLIENT_ID }}
      tenant-id: ${{ secrets.AZURE_TENANT_ID }}
      subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}