const apiBaseUrl = 'https://localhost:7136/api';

// Handle adding a new quote
document.getElementById('addQuoteForm').addEventListener('submit', function (event) {
    event.preventDefault();
    const text = document.getElementById('newQuoteText').value;
    const author = document.getElementById('newQuoteAuthor').value;
    addNewQuote(text, author);
});

// Fetch quotes and display them with tags
function fetchQuotes() {
    fetch(`${apiBaseUrl}/quotes`)
        .then(response => response.json())
        .then(data => {
            // Sort quotes by likes in descending order
            const sortedData = data.sort((a, b) => b.likes - a.likes);

            const quotesList = document.getElementById('quotes-list');
            quotesList.innerHTML = sortedData.map(quote => {
                const tags = quote.tagAssignments.map(ta => ta.tag.name).join(', '); // Convert tag assignments to a string of tag names
                return `
                <div id="quote-${quote.id}" class="quote-item">
                    <div class="quote-tags"><strong>Tags:</strong> ${tags || 'No Tags'}</div>
                    <div class="quote-text">${quote.text}</div>
                    <div class="quote-author">- ${quote.author || 'Unknown'}</div>
                    <div class="quote-actions">
                        <button onclick="likeQuote(${quote.id})">Like</button>
                        <span id="likes-counter-${quote.id}">${quote.likes}</span>
                        <button onclick="deleteQuote(${quote.id})">Delete</button>
                        <button onclick="startEditQuote(${quote.id})">Edit</button>
                    </div>
                </div>`;
            }).join('');
        })
        .catch(error => console.error('Error fetching quotes:', error));
}



// Add a new quote
function addNewQuote(text, author) {
    const tagInput = document.getElementById('quoteTags').value; // Get the tag input
    const tags = tagInput.split(',').map(tag => tag.trim()); // Create an array of tags

    fetch(`${apiBaseUrl}/quotes`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ text, author, tags }) // Include tags in the request body
    })
        .then(response => response.json())
        .then(() => {
            document.getElementById('newQuoteText').value = '';
            document.getElementById('newQuoteAuthor').value = '';
            document.getElementById('quoteTags').value = ''; // Clear the tags input field
            fetchQuotes(); // Reload quotes to see the new addition
            fetchTags(); // Reload tags to see the new addition)
        })
        .catch(error => console.error('Error adding quote:', error));
}



// Like a quote
function likeQuote(quoteId) {
    fetch(`${apiBaseUrl}/quotes/${quoteId}/like`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' }
    })
        .then(response => response.json())
        .then(updatedQuote => {
            const likesCounter = document.getElementById(`likes-counter-${quoteId}`);
            if (likesCounter) {
                likesCounter.textContent = updatedQuote.likes; // Update the likes counter in the UI
            }
        })
        .catch(error => console.error('Error liking quote:', error));
}



// Delete a quote
function deleteQuote(quoteId) {
    fetch(`${apiBaseUrl}/quotes/${quoteId}`, {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' }
    })
        .then(response => {
            if (response.ok) {
                console.log(`Quote ${quoteId} deleted successfully`);
                fetchQuotes(); // Refresh the quotes list
            } else {
                console.error(`Failed to delete quote with ID ${quoteId}`);
            }
        })
        .catch(error => console.error('Error deleting quote:', error));
}


// Edit a quote
function startEditQuote(quoteId) {
    // Fetch the existing quote and tags first
    fetch(`${apiBaseUrl}/quotes/${quoteId}`)
        .then(response => response.json())
        .then(quoteData => {
            const newText = prompt('Enter the new text for the quote:', quoteData.text);
            const newAuthor = prompt('Enter the author for the quote:', quoteData.author);
            // Join the existing tags into a string and prompt for new tags
            const existingTags = Array.isArray(quoteData.tags) ? quoteData.tags.join(', ') : '';
            const newTags = prompt('Enter the new tags:', existingTags);
            const tags = newTags.split(',').map(tag => tag.trim());

            if (newText !== null && newAuthor !== null && newTags !== null) {
                const updatePayload = { text: newText, author: newAuthor, tags };

                fetch(`${apiBaseUrl}/quotes/${quoteId}`, {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(updatePayload)
                })
                    .then(response => {
                        if (!response.ok) {
                            throw new Error(`HTTP error! status: ${response.status}`);
                        }
                        return response.json();
                    })
                    .then(() => {
                        updateQuoteInDOM(quoteId, newText, newAuthor, tags); // Update the quote in the DOM
                        console.log(`Quote ${quoteId} updated successfully`);
                    })
                    .catch(error => console.error('Error updating quote:', error));
            }
        })
        .catch(error => console.error('Error fetching quote:', error));
}

function updateQuoteInDOM(quoteId, newText, newAuthor, tags) {
    // This function would handle updating the DOM elements for the quote
    const quoteElement = document.getElementById(`quote-${quoteId}`);
    if (quoteElement) {
        quoteElement.dataset.text = newText;
        quoteElement.dataset.author = newAuthor;
        quoteElement.querySelector('.quote-text').textContent = newText;
        quoteElement.querySelector('.quote-author').textContent = newAuthor;
    }
}


// Fetch tags and display them
function fetchTags() {
    fetch(`${apiBaseUrl}/tags`)
        .then(response => response.json())
        .then(data => {
            const tagsList = document.getElementById('tags-list'); // Ensure you have a corresponding element in your HTML
            tagsList.innerHTML = data.map(tag => `<li>${tag.name}</li>`).join('');
        })
        .catch(error => console.error('Error fetching tags:', error));
}

// Search quotes by tag
function searchQuotesByTag() {
    const tagInput = document.getElementById('searchTags');
    const tag = tagInput.value.trim();
    if (!tag) {
        alert('Please enter a tag to search.');
        return;
    }
    fetch(`${apiBaseUrl}/quotes/bytag/${encodeURIComponent(tag)}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`No quotes found for tag '${tag}' or error: ${response.status}`);
            }
            return response.json();
        })
        .then(quotes => {
            displaySearchResults(quotes, tag);
        })
        .catch(error => {
            console.error(`Error fetching quotes by tag '${tag}':`, error);
            document.getElementById('searchResults').innerHTML = `Error: ${error.message}`;
        });
}


// Update the displaySearchResults function
function displaySearchResults(quotes, tag) {
    const resultsContainer = document.getElementById('searchResults');
    if (quotes.length > 0) {
        const quotesHtml = quotes.map(q => `<li>${q.text} - ${q.author || 'Unknown'}</li>`).join('');
        resultsContainer.innerHTML = `<h3>Quotes tagged with "${tag}":</h3><ul>${quotesHtml}</ul>`;
    } else {
        resultsContainer.innerHTML = `<p>No quotes found with the tag "${tag}".</p>`;
    }
}

// Add event listener to the search button instead of inline onclick attribute
document.getElementById('tagSearchForm').addEventListener('submit', function(event) {
    event.preventDefault();
    searchQuotesByTag();
});




// Call fetchQuotes on page load
fetchQuotes();

fetchTags(); // Fetch tags on page load