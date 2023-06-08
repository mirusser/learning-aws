# Elastic Compute Cloud (Infrastructure as a Service) - EC2

## Consits of:

- Renting virtual machines (EC2)
- Storing data on virtual drives (EBS)
- Distributing load across machines (ELB)
- Scaling the services using an auto-scaling group (ASG)

## Types

example: _m5.2xlarge_
m: instance type
5: generation (AWS improves them over time)
2xlarge: size within the instance class

## Security groups

### Ports

- 22 = SSH (Secure Shell) - log into a Linux instance
- 21 = FTP (File Transport Protocol) - upload files into a file share
- 22 = SFTP (Secure File Transport Protocol) - upload files using SSH
- 80 = HTTP - access unsecured websites
- 443 = HTPS - access secured websites
- 3389 = RDP (Remote Desktop Protocl) - log into a Windows instance
