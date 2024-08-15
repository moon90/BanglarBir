//const cardContainer = document.getElementById('card-container');
//let page = 1;

//function LoadVictims(page) {
//    const keyword = document.getElementById('keywordSearch').value.toLowerCase();
//    const donationNeeded = document.getElementById('donationNeeded').value.toLowerCase();
//    const status = document.getElementById('status').value.toLowerCase();

//    // Clear the container if searching with a keyword, donationNeeded, or status
//    if (keyword || donationNeeded || status) {
//        cardContainer.innerHTML = '';
//        page = 1; // Reset to the first page
//    }
//    else if (donationNeeded == '' || status == '') {
//        cardContainer.innerHTML = '';
//        page = 1;
//    }
//    else if (status == '') {
//        cardContainer.innerHTML = '';
//        page = 1;
//    }

//    fetch(`/Home/LoadVictims?page=${page}&keyword=${keyword}&donationNeeded=${donationNeeded}&status=${status}`)
//        .then(response => response.json())
//        .then(data => {
//            if (data.success && data.data.length > 0) {
//                data.data.forEach(item => {
//                    const card = document.createElement('div');
//                    card.className = 'card card-custom';
//                    card.innerHTML = `
//                        <div>
//                            <img src="${item.picture ? item.picture : 'https://via.placeholder.com/150'}" alt="Picture" class="card-img-top">
//                        </div>
//                        <div class="card-body">
//                            <h5 class="card-title">${item.name}</h5>
//                            <p class="card-text"><strong>Dead/Injured:</strong> ${item.status}</p>
//                            <p class="card-text"><strong>Phone No.:</strong> ${item.phone}</p>
//                            <p class="card-text"><strong>bKash No.:</strong> ${item.bKashNumber}</p>
//                            <p class="card-text"><strong>Donation Needed:</strong> ${item.donationNeeded}</p>
//                            <p class="card-text"><strong>BDT Donation Confirmed:</strong> ${item.donationConfirmed}</p>
//                            <p class="card-text"><strong>Location:</strong> ${item.location}</p>
//                            <p class="card-text"><strong>Student:</strong> ${item.isStudent}</p>
//                            <p class="card-text"><strong>Volunteer:</strong> <a href="volunteers/${item.volunteerId}" target="_blank">${item.volunteerName}</a></p>
//                        </div>
//                    `;
//                    cardContainer.appendChild(card);
//                });

//                // If there are no more records, stop loading more pages
//                if (!data.hasMoreRecords) {
//                    cardContainer.removeEventListener('scroll', handleScroll);
//                }
//            } else {
//                // If no results are found
//                if (page === 1) {
//                    cardContainer.innerHTML = `<p>No victims found matching your criteria.</p>`;
//                }
//            }
//        })
//        .catch(error => console.error('Error loading cards:', error));
//}

//function handleScroll() {
//    const { scrollTop, scrollHeight, clientHeight } = cardContainer;
//    if (scrollTop + clientHeight >= scrollHeight - 5) {
//        page++;
//        LoadVictims(page);
//    }
//}

//// Add event listeners for search fields to clear and reload data on search
//document.getElementById('keywordSearch').addEventListener('input', () => LoadVictims(1));
//document.getElementById('donationNeeded').addEventListener('change', () => LoadVictims(1));
//document.getElementById('status').addEventListener('change', () => LoadVictims(1));

//cardContainer.addEventListener('scroll', handleScroll);
//LoadVictims(page);

const cardContainer = document.getElementById('card-container');
const paginationContainer = document.getElementById('pagination-container');
let page = 1;
let totalRecords = 0;
let pageSize = 12;

function LoadVictims(page) {
    const keyword = document.getElementById('keywordSearch').value.toLowerCase();
    const donationNeeded = document.getElementById('donationNeeded').value.toLowerCase();
    const status = document.getElementById('status').value.toLowerCase();

    fetch(`/Home/LoadVictims?page=${page}&pageSize=${pageSize}&keyword=${keyword}&donationNeeded=${donationNeeded}&status=${status}`)
        .then(response => response.json())
        .then(data => {
            if (data.success && data.data.length > 0) {
                cardContainer.innerHTML = ''; // Clear previous cards
                data.data.forEach(item => {
                    const card = document.createElement('div');
                    card.className = 'col-lg-3 col-md-4 col-sm-6 mb-4'; // Responsive column
                    card.innerHTML = `
                        <div class="card shadow-sm h-100">
                            <img src="${item.picture ? item.picture : 'https://via.placeholder.com/150'}" alt="Picture" class="card-img-top">
                            <div class="card-body">
                                <h5 class="card-title" style="color: #28a745;">${item.name}</h5> <!-- Title in green -->
                                <p class="card-text"><strong>Status:</strong> <span style="color: ${item.status === 'Dead' ? '#ff4d4d' : '#28a745'};">${item.status}</span></p> <!-- Status color -->
                                <p class="card-text"><strong>Phone:</strong> ${item.phone}</p>
                                <p class="card-text"><strong>bKash:</strong> ${item.bKashNumber}</p>
                                <p class="card-text"><strong>Needed:</strong> ${item.donationNeeded}</p>
                                <p class="card-text"><strong>Confirmed:</strong> ${item.donationConfirmed}</p>
                                <p class="card-text"><strong>Location:</strong> ${item.location}</p>
                                <p class="card-text"><strong>Student:</strong> ${item.isStudent ? 'Yes' : 'No'}</p>
                                <p class="card-text"><strong>Volunteer:</strong> <a href="volunteers/${item.volunteerId}" target="_blank">${item.volunteerName}</a></p>
                            </div>
                        </div>
                    `;
                    cardContainer.appendChild(card);
                });

                // Set up pagination
                totalRecords = data.totalRecords;
                pageSize = data.pageSize;
                page = data.currentPage
                setupPagination(page, Math.ceil(totalRecords / pageSize));
            } else {
                cardContainer.innerHTML = `<p>No victims found matching your criteria.</p>`;
                paginationContainer.innerHTML = ''; // Clear pagination
            }
        })
        .catch(error => console.error('Error loading cards:', error));
}

function setupPagination(currentPage, totalPages) {
    paginationContainer.innerHTML = ''; // Clear existing pagination

    // Create First button
    const firstButton = document.createElement('button');
    firstButton.className = 'btn mx-1';
    firstButton.style.backgroundColor = '#28a745'; // Green
    firstButton.style.color = '#fff';
    firstButton.innerText = 'First';
    firstButton.disabled = currentPage === 1;
    firstButton.onclick = () => LoadVictims(1);
    paginationContainer.appendChild(firstButton);

    // Create Previous button
    const prevButton = document.createElement('button');
    prevButton.className = 'btn mx-1';
    prevButton.style.backgroundColor = '#28a745'; // Green
    prevButton.style.color = '#fff';
    prevButton.innerText = 'Previous';
    prevButton.disabled = currentPage === 1;
    prevButton.onclick = () => LoadVictims(currentPage - 1);
    paginationContainer.appendChild(prevButton);

    // Create numbered page buttons
    const maxVisiblePages = 5;
    let startPage = Math.max(currentPage - Math.floor(maxVisiblePages / 2), 1);
    let endPage = Math.min(startPage + maxVisiblePages - 1, totalPages);

    if (endPage - startPage < maxVisiblePages - 1) {
        startPage = Math.max(endPage - maxVisiblePages + 1, 1);
    }

    for (let i = startPage; i <= endPage; i++) {
        const pageButton = document.createElement('button');
        pageButton.className = `btn mx-1 ${i === currentPage ? 'active' : ''}`;
        pageButton.style.backgroundColor = i === currentPage ? '#ff4d4d' : '#28a745'; // Active button in red, others in green
        pageButton.style.color = '#fff';
        pageButton.innerText = i;
        pageButton.onclick = () => LoadVictims(i);
        paginationContainer.appendChild(pageButton);
    }

    // Create Next button
    const nextButton = document.createElement('button');
    nextButton.className = 'btn mx-1';
    nextButton.style.backgroundColor = '#28a745'; // Green
    nextButton.style.color = '#fff';
    nextButton.innerText = 'Next';
    nextButton.disabled = currentPage === totalPages;
    nextButton.onclick = () => LoadVictims(currentPage + 1);
    paginationContainer.appendChild(nextButton);

    // Create End button
    const endButton = document.createElement('button');
    endButton.className = 'btn mx-1';
    endButton.style.backgroundColor = '#28a745'; // Green
    endButton.style.color = '#fff';
    endButton.innerText = 'End';
    endButton.disabled = currentPage === totalPages;
    endButton.onclick = () => LoadVictims(totalPages);
    paginationContainer.appendChild(endButton);
}

// Add event listeners for search fields
document.getElementById('keywordSearch').addEventListener('input', () => LoadVictims(1));
document.getElementById('donationNeeded').addEventListener('change', () => LoadVictims(1));
document.getElementById('status').addEventListener('change', () => LoadVictims(1));

// Initial load
LoadVictims(page);







function search() {
    const keyword = document.getElementById('keywordSearch').value.toLowerCase();
    const donationNeeded = document.getElementById('donationNeeded').value.toLowerCase();
    const status = document.getElementById('status').value.toLowerCase();

    // Assuming you have a fetch or AJAX call to get the filtered data from the server
    fetch(`/Home/Filter?keyword=${keyword}&donationNeeded=${donationNeeded}&status=${status}`)
        .then(response => response.json())
        .then(data => {
            displayResults(data);
        })
        .catch(error => console.error('Error fetching data:', error));
}

function displayResults(data) {
    const resultsContainer = document.getElementById('resultsContainer');
    resultsContainer.innerHTML = ''; // Clear previous results

    if (data.length === 0) {
        resultsContainer.innerHTML = '<p class="text-center">No results found</p>';
        return;
    }

    data.forEach(item => {
        const card = document.createElement('div');
        card.className = 'card card-custom';
        card.innerHTML = `
            <img src="${item.picture}" class="card-img-top" alt="Victim Image">
            <div class="card-body">
                <h5 class="card-title">${item.name}</h5>
                <p class="card-text"><strong>Dead/Injured:</strong> ${item.status}</p>
                <p class="card-text"><strong>Phone No.:</strong> ${item.phone}</p>
                <p class="card-text"><strong>bKash No.:</strong> ${item.bKashNumber}</p>
                <p class="card-text"><strong>Donation Needed:</strong> ${item.donationNeeded}</p>
                <p class="card-text"><strong>BDT Donation Confirmed:</strong> ${item.donationConfirmed}</p>
                <p class="card-text"><strong>Location:</strong> ${item.location}</p>
                <p class="card-text"><strong>Student:</strong> ${item.isStudent}</p>
                <p class="card-text"><strong>Volunteer:</strong> <a href="volunteers/edit/1${item.id}" target="_blank">${item.volunteerName}</a></p>
            </div>
        `;
        resultsContainer.appendChild(card);
    });
}
