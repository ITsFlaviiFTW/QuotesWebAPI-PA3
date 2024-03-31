import requests
import urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)


# The URL for my Quotes Web API
api_base_url = 'https://localhost:7136/api'  
quotes_url = f'{api_base_url}/quotes'

def load_quotes_from_file(file_path):
    with open(file_path, 'r', encoding='utf-8-sig') as file:
        quotes = [line.strip() for line in file.readlines()]
    
    for quote in quotes:
        parts = quote.split(" -- ")  # Correctly split the line into quote and author
        if len(parts) == 2:
            text, author = parts
        else:
            text = parts[0]  # Assume the entire line is the quote if no author is found
            author = None

        payload = {'text': text.strip(), 'author': author.strip() if author else None}
        response = requests.post(quotes_url, json=payload, verify=False)
        if response.status_code == 201:
            print(f'Quote "{text[:50]}..." added successfully')
        else:
            print(f'Failed to add quote "{text[:50]}...". Status code: {response.status_code}')

if __name__ == "__main__":
    load_quotes_from_file('quotes.txt')
