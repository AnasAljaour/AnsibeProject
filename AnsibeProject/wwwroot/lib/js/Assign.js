let allocation = [];


function getAllocationBasedOnYear(selected) {
    let year = selected.value;

    $.ajax({
        url: '/AssignP/getAnsibeByYear',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ year: year }),
        success: function (response) {
            success(response);
        },
        error: function (xhr, status, error) {
            failed(xhr, status, error);

        }
    });

}

function success(response) {
    
    let tbody = document.getElementById('Ansibe-tbody');
    tbody.innerHTML = '';

    for (let i = 0; i < response.length; i++) {
        row = document.createElement("tr");
        idCell = document.createElement('td');
        idCell.textContent = response[i].id;
        row.append(idCell);
        yearCell = document.createElement("td");
        yearCell.textContent = response[i].year;
        row.append(yearCell);


        let button = document.createElement("Button");
        button.textContent = "use";
        button.addEventListener("click", function () {
            showAllocation(button);

        });
        button.classList.toggle("inline-btn");
        buttonCell = document.createElement('td');
        buttonCell.appendChild(button);
        row.append(buttonCell);


        tbody.appendChild(row);
    }
}

function failed(jqXHR, textStatus, errorThrown) {

    if (jqXHR.responseJSON && jqXHR.responseJSON.error) {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: "Something went wrong! \n" + jqXHR.responseJSON.error,

        });
    } else if (jqXHR.status === 400) {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: "Something went wrong! \n" + jqXHR.responseText,

        });
    } else {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: "Something went wrong! \n" + jqXHR.responseText,

        });
    }
}

let type;
function showAllocation(button) {
    let row = button.closest('tr');
    let columns = row.getElementsByTagName('td');
    // selected Id from list
    usedId = {
        Key: "Id",
        Value: columns[0].textContent
    }
    //created Id for this Ansibe
    newAnsibe = {
        Key: "Id",
        Value: document.getElementById('AnsibeId').value
    }
    Swal.fire({
        title: "Are you sure you want to use this ? exsiting work will be lost ",
        text: "",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes!"
    }).then((result) => {
        if (result.isConfirmed) {
            //delete from database and then get the new view based on selected Ansibe ID
            deleteSectionsFromDataBase(usedId,newAnsibe)
            

        } else {
            // If user cancels, show an informational message
            Swal.fire("cancelled", "", "info");
        }
    });





    


}

function getPartialViewBasedOnSelectedId(usedId, newAnsibe) {
    
    
    $.ajax({
        url: '/AssignP/getSectionOfTheAnsibeById',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ AnsibeId: usedId, NewAnsibeId: newAnsibe }),
        dataType: 'html',
        success: function (response) {

            document.getElementById('holder').innerHTML = response;
            showSidebar()
            type="CP"
            
        },
        error: function (xhr, status, error) {
            failed(xhr, status, error);

        }
    });
}

function deleteSectionsFromDataBase(usedId,newAnsibe) {
    $.ajax({
        url: '/AssignP/DeleteSectionsOfAnsibe',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(newAnsibe),
        dataType: 'json',
        success: function (response) {
            getPartialViewBasedOnSelectedId(usedId, newAnsibe);
        },
        error: function (xhr, status, error) {
            failed(xhr, status, error);
            

        }
    });
}
alert("hello");
function swapContent() {
    let AnsibeId = {
        Key: "Id",
        Value: document.getElementById('AnsibeId').value
    }
    if (type !== null) {

        type = (type === "CP") ? "PS" : "CP";
        $.ajax({
            url: '/AssignP/ToggleView',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ AnsibeId: AnsibeId, Allocation: allocation, Type: type }),
            dataType: 'html',
            success: function (response) {

                document.getElementById('holder').innerHTML = response;


            },
            error: function (xhr, status, error) {
                failed(xhr, status, error);


            }
        });
    }
}






function showCreateSection() {
    let popup = document.getElementById('popup');
    popup.classList.toggle('open-popup');

}
let tempSections = []

function createSections(button, CourseCode, CourseDescription, CourseHours, TP, TD) {
    var model = {
        SectionId: 0,
        Course: {
            CourseCode: CourseCode
        },
        Professor: null,
        TP: undefined,
        TD: undefined,
        CourseHours: undefined,
        Language: undefined
    };

    let row = button.closest("tr");
    let checkboxes = row.querySelectorAll('input[type="checkbox"]');
   
    let option = row.querySelector('select');
    var selectedText = option.options[option.selectedIndex].textContent;

    model.Language = option.selectedIndex

    let checkedCheckboxes = [];
    let i = 0;
    checkboxes.forEach(function (checkbox) {
        checkedCheckboxes.push(checkbox.checked);

        if (checkbox.checked) {
            i++;
            checkbox.checked = false;
        }
    });

    model.CourseHours = (checkedCheckboxes[0]) ? parseInt(CourseHours) : null;
    model.TD = (checkedCheckboxes[1]) ? parseInt(TD) : null;
    model.TP = (checkedCheckboxes[2]) ? parseInt(TP) : null;


    if (i > 0) {
        let newRow = document.createElement('tr');

        let courseCell = document.createElement('td');
        courseCell.textContent = model.CourseHours;

        let TPCell = document.createElement('td');
        TPCell.textContent = model.TP;

        let TDCell = document.createElement('td');
        TDCell.textContent = model.TD;

        let languageCell = document.createElement('td');
        languageCell.textContent = selectedText

        let deleteButtonCell = document.createElement('td');
        let deleteButton = document.createElement('button');
        deleteButton.type = "button";
        deleteButton.textContent = "delete";
        deleteButton.onclick = function () {

        };

        deleteButton.classList.toggle("inline-delete-btn");
        deleteButtonCell.appendChild(deleteButton);


        newRow.appendChild(courseCell);
        newRow.appendChild(TPCell);
        newRow.appendChild(TDCell);
        newRow.appendChild(languageCell);
        newRow.appendChild(deleteButtonCell);

        let tbody = document.getElementById('tbody-' + model.Course.CourseCode);
        tbody.appendChild(newRow);
        tempSections.push(model);


    }
    else {
        Swal.fire("at least one checkbox should be checked", "", "info");

    }

}

function cancelCreation() {
    let popup = document.getElementById('popup');
    popup.classList.toggle('open-popup');
    tempSections = [];

    let tbodyElements = popup.querySelectorAll('tbody[id^="tbody-"]');
    tbodyElements.forEach(function (tbody) {
        tbody.innerHTML = '';
    });
}

function saveCreatedSections() {
    if (tempSections.length > 0) {
        let AnsibeId = document.getElementById('AnsibeId').value
        $.ajax({
            url: '/AssignP/SaveCreatedSections',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({TempSections: tempSections, Type: type, AnsibeId: AnsibeId }),
            dataType: 'html',
            success: function (response) {

                document.getElementById('holder').innerHTML = response;
                
            },
            error: function (xhr, status, error) {
                failed(xhr, status, error);

                


            }
        });
        if (type == null) type = "CP";
        cancelCreation()


    }
    else {
        Swal.fire("at least one section should be created", "", "info");
    }

}



function cancelAssignProfessorSection() {
    let popup = document.getElementById('popup1');
    popup.classList.toggle('open-popup');
    let checkboxes = popup.querySelectorAll('input[type="checkbox"]');
    checkboxes.forEach(function (checkbox) {
        checkbox.checked = false;
    });
}

let TdBtn
let sectionID
function showAssignProfessors(button) {
    TdBtn = button.closest("td");
    var row = TdBtn.closest("tr");
    var firstrowID = row.querySelector('input[type="hidden"]')
    sectionID = firstrowID.value;
    var popup = document.getElementById("popupProfessors");
    popup.classList.toggle("open-popup");
}

function AssignProfessorInCourse(button) {
    var btnAssignProfessor = button.closest("tr");

    var firstCell = btnAssignProfessor.querySelector("td:first-child");

    TdBtn.textContent = firstCell.nextElementSibling.textContent;

    /*var section = sections.find(s => s.SectionId === sectionID.parseInt);
   
    if (section) 
        section.professor = { fileNumber: parseInt(firstCell.textContent.trim()) };
    */
    var professoreId = parseInt(firstCell.textContent.trim());
    addCourse(professoreId, sectionID)

    var popup = document.getElementById("popupProfessors");
    popup.classList.toggle("open-popup");
    console.log(allocation);
}






let prof


function showAssignSections(button) {
    var row = button.closest("tr");
    var firstrowID = row.querySelector('input[type="hidden"]')
    prof = firstrowID.value;
    console.log(prof);
    var popup = document.getElementById("popup1");
    popup.classList.toggle("open-popup");

}
function saveProfessorAssign() {
    var tbody = document.getElementById("SectionId");
    var checkboxes = tbody.querySelectorAll('input[type="checkbox"]');


    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) {
            // Remove the parent row of the checked checkbox
            var row = checkbox.closest('tr');
            console.log(prof);

            var newRow = prepareRow(row);
            row.parentNode.removeChild(row);
            let courseTable = document.getElementById("tbody-" + prof);
            courseTable.appendChild(newRow);
        }
    });

    var popup = document.getElementById("popup1");
    popup.classList.toggle("open-popup");

    console.log(allocation);

}



function prepareProfessorRow(row) {
    let newRow = document.createElement("tr");
    let columns = row.getElementsByTagName("td");
    var sectionId = row.querySelector('input[type="hidden"]').value;

    let cloneCell = columns[0].cloneNode(true);
    newRow.appendChild(cloneCell);
    for (let i = 1; i < columns.length - 1; i++) {
        
        var newcell = document.createElement("td");
        newcell.textContent = columns[i].textContent

        newRow.appendChild(newcell);


    }
    let cellForButton = document.createElement("td");


    let btn = document.createElement("button")
    btn.type = "button";
    btn.textContent = "delete"
    btn.onclick = function () {
       // confirmDeleteAssignement(this);
    };
    btn.classList.toggle("inline-delete-btn");
    cellForButton.appendChild(btn);
    newRow.appendChild(cellForButton);

    addCourse(prof, sectionId);
    return newRow
}
function addCourse(professoreId, sectionId) {
    let model = {
        Key: sectionId.toString(),
        Value: professoreId.toString()
    };
    allocation.push(model);

}
function prepareRow(row) {
    var sectionId = row.querySelector('input[type="hidden"]').value;
    let newRow = row.cloneNode(true);
    let lastCell = newRow.cells[newRow.cells.length - 1];
    lastCell.innerHTML = '<input type="button" class="inline-delete-btn" value="delete"/>';
    addCourse(prof, sectionId);
    return newRow
}


function showSidebar() {
    let sideBar = document.querySelector('.side-bar');
    sideBar.classList.toggle('active');
    var s = document.getElementById("sectionAssign")
    s.classList.toggle('Assing');
}



