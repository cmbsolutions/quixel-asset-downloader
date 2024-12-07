((async (startPage = 0, autoClearConsole = true) => {

  const getCookie = (name) => {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
  }

  const callCacheApi = async (params = {}) => {
    const defaultParams = {
      page: 0,
      maxValuesPerFacet: 1000,
      hitsPerPage: 1000,
      attributesToRetrieve: ["id", "name"].join(",")
    }
    const response = await fetch("https://proxy-algolia-prod.quixel.com/algolia/cache", {
      "headers": {
        "x-api-key": "2Zg8!d2WAHIUW?pCO28cVjfOt9seOWPx@2j"
      },
      "body": JSON.stringify({
        url: "https://6UJ1I5A072-2.algolianet.com/1/indexes/assets/query?x-algolia-application-id=6UJ1I5A072&x-algolia-api-key=e93907f4f65fb1d9f813957bdc344892",
        params: new URLSearchParams({ ...defaultParams, ...params }).toString()
      }),
      "method": "POST",
    })
    return await response.json()
  }

  const callAcl = async ({ id, name }) => {
    const response = await fetch("https://quixel.com/v1/acl", {
      "headers": {
        "authorization": "Bearer " + authToken,
        "content-type": "application/json;charset=UTF-8",
      },
      "body": JSON.stringify({ assetID: id }),
      "method": "POST",
    });
    const json = await response.json()
    if (json?.isError) {
      console.error(`  --> **UNABLE TO ADD ITEM** Item ${id} | ${name} (${json?.msg})`)
    } else {
      console.log(`  --> ADDED ITEM Item ${id} | ${name}`)
    }
  }

  const callAcquired = async () => {
    const response = await fetch("https://quixel.com/v1/assets/acquired", {
      "headers": {
        "authorization": "Bearer " + authToken,
        "content-type": "application/json;charset=UTF-8",
      },
      "method": "GET",
    });
    return await response.json()
  }

  // 1. Check token exist, quixel API needs it
  console.log("-> Checking Auth API Token...")
  let authToken = ""
  try {
    const authCookie = getCookie("auth") ?? "{}"
    authToken = JSON.parse(decodeURIComponent(authCookie))?.token
    if (!authToken) {
      return console.error("-> Error: cannot find authentication token. Please login again.")
    }
  } catch (_) {
    return console.error("-> Error: cannot find authentication token. Please login again.")
  }

  // 2. Get all currently acquired items
  console.log("-> Get Acquired Items...")
  const acquiredItems = (await callAcquired()).map(a => a.assetID)

  // 3. Get total count of items
  console.log("-> Getting Total Number of Pages....")
  const { nbPages: totalPages, hitsPerPage: itemsPerPage, nbHits: totalItems } = await callCacheApi()


  console.log("-> ==============================================")
  console.log(`-> Total # of items: ${totalItems}`)
  console.log(`-> ${totalPages} total pages with ${itemsPerPage} per page`)
  console.log(`-> Total Items to add: ${(totalItems - acquiredItems.length)}.`)
  console.log("-> ==============================================")

  if (!confirm(`Click OK to start adding ${(totalItems - acquiredItems.length)} items in your account.`)) return

  // Loop
  for (let pageIdx = startPage || 0; pageIdx < totalPages; pageIdx++) {
    console.log(`-> ======================= PAGE ${pageIdx + 1}/${totalPages} START =======================`)

    console.log("-> Getting Items from page " + (pageIdx + 1) + " ...")

    const { hits: items } = await callCacheApi({ page: pageIdx })

    console.log("-> Adding non-acquired items...")

    // Filter out owned items
    const unownedItems = items.filter(i => !acquiredItems.includes(i.id))
    const aclPromises = unownedItems.map(callAcl)

    await Promise.all(aclPromises)
    console.log(`-> ======================= PAGE ${pageIdx + 1}/${totalPages} COMPLETED =======================`)
    if (autoClearConsole) console.clear() // Fix the issue that too much log hangs the console. Set autoClearConsole = false to keep the logs
  }

  console.log("-> Getting new acquired info...")
  // Get acquired items again
  const newItemsAcquired = (await callAcquired()).length
  const newTotalCount = (await callCacheApi()).nbHits

  console.log(`-> Completed. Your account now have a total of ${newItemsAcquired} out of ${newTotalCount} items.`)

  alert(`-> Your account now have a total of ${newItemsAcquired} out of ${newTotalCount} items.\n\nIf you find some items missing, try refresh the page and run the script again.`)
})())
