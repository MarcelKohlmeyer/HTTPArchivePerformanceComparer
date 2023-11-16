# HTTPArchivePerformanceComparer
## General
This API takes multiple HAR-files (that can be exported from Chrome Devools) and compares the duration for each requested URL in the files.
Can for example be used to compare different (backend-)loactions or track performance improvements of APIs.

## API
### POST /httpArchives
Takes a HAR-file as ```multipart/form-data```, saves the data in memory and returns a JSON-object with the duration for each requested URL in the file.

### GET /httpArchives
Returns a list of all uploaded HAR-files.

### DELETE /httpArchives/{name}
Deletes the HAR-file with the given name from the APIs memory

### GET /httpArchives/compare
Compares all stored HAR files and returns each request-duration for every URL as well as an average duration per url-request per HAR-file.